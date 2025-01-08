
#Dados de Entrada: Horários de professores, disponibilidade de salas, restrições (e.g., uma sala para cada aula, tempos de intervalo).
#Pré-processamento: Estruturar os dados como variáveis, domínios (possíveis horários/salas) e restrições binárias.
#Implementação em Python: Usar a biblioteca constraint para modelar e resolver o problema.

#Variáveis:
#Cada aula (matéria + turma + professor) é uma variável.
#Por exemplo: Matemática_TurmaA, Português_TurmaB.

#Domínio:
#Cada variável pode assumir valores de pares (sala, horário).
#Exemplo de domínio para Matemática_TurmaA:
#{(Sala1, 9h00), (Sala2, 10h00), ..., (SalaN, 15h00)}.

#Restrições:
#Horários dos professores: Um professor não pode dar duas aulas ao mesmo tempo.
#Horários das turmas: Uma turma não pode ter duas aulas ao mesmo tempo.
#Capacidade das salas: As turmas só podem ser atribuídas a salas com capacidade suficiente.
#Preferências (opcional): Professores ou turmas podem preferir certos horários ou salas.

from collections import deque
from typing import List, Dict, Tuple, Any
import itertools
import copy
import time

class SchedulerCSP:
    def __init__(self, variables: List[str], domains: Dict[str, List[Any]], 
        constraints: List[Tuple[str, str, callable]], max_restarts: int = 5, cutoff: float = None):

        self.variables = variables  # Lista de variáveis (ex.: aulas, turmas, professores)
        self.domains = domains  # Domínios possíveis para cada variável
        self.constraints = constraints  # Lista de restrições
        self.assignments = {}  # Alocação atual das variáveis
        self.cutoff = cutoff  # O cutoff em segundos (pode ser definido por você)
        self.max_restarts = max_restarts
        self.restarts = 0  # Contador de reinícios

    def is_consistent(self, var, value):
        # Verifica se a atribuição é consistente com as restrições
        for (var1, var2, constraint) in self.constraints:
            if var1 == var and var2 in self.assignments:
                if not constraint(value, self.assignments[var2]):
                    return False
            if var2 == var and var1 in self.assignments:
                if not constraint(self.assignments[var1], value):
                    return False
        return True

    def forward_checking(self, var, value):
        # Atualiza os domínios das variáveis não atribuídas com base na consistência
        local_domains = copy.deepcopy(self.domains)
        local_domains[var] = [value]

        for (var1, var2, constraint) in self.constraints:
            if var1 == var and var2 not in self.assignments:
                local_domains[var2] = [v for v in local_domains[var2] if constraint(value, v)]
            elif var2 == var and var1 not in self.assignments:
                local_domains[var1] = [v for v in local_domains[var1] if constraint(v, value)]

        # Verifica se algum domínio ficou vazio
        for key in local_domains:
            if not local_domains[key]:
                return None
        return local_domains

    def select_unassigned_variable(self):
        # Heurística de "Menor Domínio Primeiro" (MRV - Minimum Remaining Values)
        unassigned = [var for var in self.variables if var not in self.assignments]
        
        # Filtra as variáveis com o menor número de valores no seu domínio (MRV)
        min_remaining_values = min(len(self.domains[var]) for var in unassigned)
        candidates = [var for var in unassigned if len(self.domains[var]) == min_remaining_values]
        
        # Se houver empate, aplica a heurística de "Degree" (máximo de restrições pendentes)
        if len(candidates) > 1:
            return max(candidates, key=lambda var: len([c for c in self.constraints if (c[0] == var or c[1] == var) and (c[0] not in self.assignments and c[1] not in self.assignments)]))
        
        # Caso contrário, retorna a variável com MRV
        return candidates[0]


    def backtrack(self, start_time):
        if len(self.assignments) == len(self.variables):
            return self.assignments

        # Verifica se o tempo de execução atingiu o cutoff
        if self.cutoff is not None and (time.perf_counter() - start_time) >= self.cutoff:
            print("\n>>> Tempo de execução atingiu o limite de Cutoff!")
            return None  # Interrompe a busca se o cutoff for atingido
        
        if self.restarts >= self.max_restarts:
            print(f"\n>>> Limite de reinícios ({self.max_restarts}) atingido.")
            return None

        var = self.select_unassigned_variable()
        original_domains = copy.deepcopy(self.domains)

        for value in self.domains[var]:
            if self.is_consistent(var, value):
                self.assignments[var] = value
                local_domains = self.forward_checking(var, value)

                if local_domains:
                    self.domains = local_domains
                    # Aplica o AC-3 após atribuição de uma variável
                    if self.ac3():
                        result = self.backtrack(start_time)
                        if result:
                            return result

                del self.assignments[var]
                self.domains = original_domains


        # Restart logic: Reiniciar a busca se falhar
        self.restarts += 1
        print(f">>> Reiniciando a busca. Tentativa: {self.restarts}/{self.max_restarts}")
        self.assignments = {}  # Resetar as atribuições
        return self.backtrack(start_time)  # Reinicia a busca

    

    def ac3(self):
        # Implementação do AC-3 para garantir consistência de arco
        queue = deque(self.constraints)  # Adiciona todas as restrições na fila
        while queue:
            var1, var2, constraint = queue.popleft()
            if self.revise(var1, var2, constraint):
                if len(self.domains[var1]) == 0:
                    return False  # Se algum domínio se tornar vazio, não há solução
                # Propaga as mudanças para outras variáveis relacionadas
                for (var3, _, _) in self.constraints:
                    if var3 != var2 and var3 != var1:
                        queue.append((var3, var1, constraint))
        return True

    def revise(self, var1, var2, constraint):
        # Função que revisa o domínio de var1 em relação a var2
        revised = False
        for value1 in self.domains[var1]:
            # Se não houver nenhum valor viável em var2 que satisfaça a restrição, remova o valor de var1
            if not any(constraint(value1, value2) for value2 in self.domains[var2]):
                self.domains[var1].remove(value1)
                revised = True
        return revised

#######################################################################################################################

def generate_domains(professores: Dict[str, List[str]], 
                     horarios_manha: List[str], horarios_tarde: List[str], 
                     dias_da_semana: List[str], class_room: Dict[str, str], 
                     turmas_manha: List[str], turmas_tarde: List[str]):

    domains = {}
    for professor, aulas in professores.items():
        for aula in aulas:
            if aula not in domains:
                domains[aula] = []
            turma = aula.split('_')[1]  # Identifica a turma a partir do nome da aula, ex: 'ET10A'
            sala_fixa = class_room.get(turma, None)  # Obtém a sala fixa para a turma

            # Define os horários disponíveis para a turma (manhã + tarde se for aplicável)
            horarios_disponiveis = []
            if turma in turmas_tarde:  # Adiciona horários da tarde apenas para turmas específicas
                horarios_disponiveis = horarios_tarde
            else:
                horarios_disponiveis = horarios_manha
            # Se houver uma sala fixa para a turma, restringe a sala ao valor correspondente
            if sala_fixa:
                for horario, dia in itertools.product(horarios_disponiveis, dias_da_semana):
                    domains[aula].append((sala_fixa, professor, horario, dia))  # Sala fixa para a turma
            else:
                # Caso a turma não tenha uma sala fixa, permite todas as combinações de salas
                for horario, dia in itertools.product(horarios_disponiveis, dias_da_semana):
                    domains[aula].append((None, professor, horario, dia))  # Sem sala fixa

    return domains


classes = {
    "ET10A": ["Matemática_ET10A", "Física_ET10A", "Química_ET10A", 
            "FAI_ET10A", "Português_ET10A", "Inglês_ET10A", 
            "EE_ET10A", "TTL_ET10A", "PO_ET10A", 
            "Informática_ET10A"],

    "ET10B": ["Matemática_ET10B", "Física_ET10B", "Química_ET10B", 
            "FAI_ET10B", "Português_ET10B", "Inglês_ET10B", 
            "EE_ET10B", "TTL_ET10B", "PO_ET10B", 
            "Informática_ET10B"],

    "ET10C": ["Matemática_ET10C", "Física_ET10C", "Química_ET10C", 
            "FAI_ET10C", "Português_ET10C", "Inglês_ET10C", 
            "EE_ET10C", "TTL_ET10C", "PO_ET10C", 
            "Informática_ET10C"],
    
    "ET11A": ["Matemática_ET11A", "Física_ET11A", "Química_ET11A", 
            "FAI_ET11A", "Português_ET11A", "Inglês_ET11A", 
            "EE_ET11A", "TTL_ET11A", "PO_ET11A", "SD_ET11A"],
    
    "ET11B": ["Matemática_ET11B", "Física_ET11B", "Química_ET11B", 
            "FAI_ET11B", "Português_ET11B", "Inglês_ET11B", 
            "EE_ET11B", "TTL_ET11B", "PO_ET11B", "SD_ET11B"],

    "ET12A": ["Matemática_ET12A", "Física_ET12A", "IT_ET12A", 
            "PT_ET12A", "EE_ET12A", "TTL_ET12A", "TEL_ET12A", 
            "PO_ET12A", "SD_ET12A"],
    
    "ET12B": ["Matemática_ET12B", "Física_ET12B", "IT_ET12B", 
            "PT_ET12B", "EE_ET12B", "TTL_ET12B", "TEL_ET12B", 
            "PO_ET12B", "SD_ET12B"],


    "I10A": ["Matemática_I10A", "Física_I10A", "Química_I10A", 
            "FAI_I10A", "Português_I10A", "Inglês_I10A", 
            "TIC_I10A", "SEAC_I10A", "LP_I10A"],

    "I10B": ["Matemática_I10B", "Física_I10B", "Química_I10B", 
            "FAI_I10B", "Português_I10B", "Inglês_I10B", 
            "TIC_I10B", "SEAC_I10B", "LP_I10B"],

    "I11A": ["Matemática_I11A", "Física_I11A", "Química_I11A", 
            "FAI_I11A", "Português_I11A", "Inglês_I11A", 
            "SEAC_I11A", "LP_I11A"],
    
    "I11B": ["Matemática_I11B", "Física_I11B", "Química_I11B", 
            "FAI_I11B", "Português_I11B", "Inglês_I11B", 
            "SEAC_I11B", "LP_I11B"],

    "I12A": ["Matemática_I12A", "Física_I12A", 
            "PT_I12A", "IT_I12A", "LP_I12A",
            "SEAC_I12A"],


    "ET13A": ["Estágio_ET13A"],

    "I13A": ["Estágio_I13A"]
}

# Dados iniciais
class_room = {
    "ET10A": "SALA1",
    "ET10B": "SALA2",
    "ET10C": "SALA3",
    "ET11A": "SALA11",
    "ET11B": "SALA12",

    "I10A": "SALA5",
    "I10B": "SALA6",
    "I11A": "SALA14",
    "I11B": "SALA15",

    "ET12A": "SALA1",
    "ET12B": "SALA2",
    "ET13A": "SALA12",

    "I12A": "SALA4",
    "I12B": "SALA5",
    "I13A": "SALA11",
}
professores = {
    "Lula Kreiger": ["Matemática_ET10A", "Matemática_ET10B", "Matemática_ET10C", 
                     "Matemática_ET11A", "Matemática_ET11B"],
    
    "Mildred Grimes": ["Matemática_ET12A", "Matemática_ET12B", "Matemática_I12A"],

    "Peggy Hauck": ["Matemática_I10A", "Matemática_I10B", 
                    "Matemática_I11A", "Matemática_I11B"],
    
    "Clifton Bednar": ["Física_ET10A", "Física_ET10B", "Física_ET10C", 
                        "Física_ET11A", "Física_ET11B"],
    
    "Norman Mayert": ["Física_ET12A", "Física_ET12B", "Física_I12A"],
    
    "Christina Stokes": ["Física_I10A", "Física_I10B", 
                         "Física_I11A", "Física_I11B"],

    "Rose Becker": ["Química_ET10A", "Química_ET10B", "Química_ET10C", 
                    "Química_ET11A", "Química_ET11B",],

    "Carrie Heaney": ["Química_I10A", "Química_I10B", 
                      "Química_I11A", "Química_I11B"],

    "Maria Emília": ["FAI_ET10A", "FAI_ET10B", "FAI_ET10C", 
                     "FAI_ET11A", "FAI_ET11B"],
    
    "Cassandra Franey": ["FAI_I10A", "FAI_I10B", 
                         "FAI_I11A", "FAI_I11B"],

    "Jared Sauer": ["Português_ET10A", "Português_ET10B", "Português_ET10C", 
                    "Português_ET11A", "Português_ET11B"],
    
    "Kay Schamberger": ["Português_I10A", "Português_I10B", 
                        "Português_I11A", "Português_I11B"],

    "Minnie Paucek": ["Inglês_ET10A", "Inglês_ET10B", "Inglês_ET10C",
                      "Inglês_ET11A", "Inglês_ET11B"],
    
    "Suzanne Stoltenberg": ["IT_ET12A", "IT_ET12B", "IT_I12A"],
    
    "Francisco Hahn": ["Inglês_I10A", "Inglês_I10B", 
                       "Inglês_I11A", "Inglês_I11B"],



    "Bethany Bergstrom": ["Informática_ET10A", "TIC_I10A",
                          "Informática_ET10B", "Informática_ET10C", 
                          "TIC_I10B"],

    "Alexandra Jerde DVM": ["SEAC_I10A", "SEAC_I10B",
                            "SEAC_I11A", "SEAC_I11B"
                            "SEAC_I12A"],

    "Keith Towne": ["LP_I10A", "LP_I10B", 
                    "LP_I11A", "LP_I11B",
                    "LP_I12A"],



    "Tammy McKenzie": ["EE_ET10A", "EE_ET10B", "EE_ET10C", 
                       "EE_ET11A", "EE_ET11B"],

    "Krista Welch": ["EE_ET12A", "EE_ET12B",
                     "PO_ET12A", "PO_ET12B"],

    "Shawna Blockerzog": ["TTL_ET10A", "TTL_ET10B", "TTL_ET10C", 
                          "TTL_ET11A", "TTL_ET11B"],

    "Isabel Hoppe": ["PO_ET10A", "PO_ET10B", "PO_ET10C", 
                     "PO_ET11A", "PO_ET11B"],

    "Mário Igreja": ["SD_ET11A", "SD_ET11B", 
                     "SD_ET12A", "SD_ET12B"],

    "Natasha Brakus": ["TTL_12A", "TTL_12B"
                       "TEL_ET12A", "TEL_ET12B"],
    
    "Denise Wolff": ["PT_ET12A", "PT_ET12B", 
                    "PT_I12A"],

    "Jessie Cassin": ["Estágio_ET13A", "Estágio_I13A"]
}


dias_da_semana = ["Segunda-feira", "Terça-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira"]
horarios_manha = ["07:00-07:50", "08:00-08:50", "09:00-09:50", "10:00-10:50", "11:00-11:50", "12:00-12:50"]
horarios_tarde = ["13:00-13:50", "14:00-14:50", "15:00-15:50", "16:00-16:50", "17:00-17:50", "18:00-18:50"]

turmas_manha = ["ET10A", "ET10B", "ET11A", "ET11B", 
                "I10A", "I10B", "I11A", "I11B"]

turmas_tarde = ["ET12A", "ET12B", "ET13A", "I12A", "I13A"]

# Gerar domínios automaticamente
start_time = time.perf_counter()
domains = generate_domains(professores, horarios_manha, horarios_tarde, dias_da_semana, 
                           class_room, turmas_manha, turmas_tarde)
variables = list(domains.keys())

# Restrição de sala única por horário e dia
def sala_unica_por_horario(val1, val2):
    return val1[0] != val2[0] or val1[2] != val2[2] or val1[3] != val2[3]

# Restrição de professor único por horário e dia
def professor_unico_por_horario(val1, val2):
    return val1[1] != val2[1] or val1[2] != val2[2] or val1[3] != val2[3]


constraints = []
for var1, var2 in itertools.combinations(variables, 2):
    constraints.append((var1, var2, sala_unica_por_horario))
    constraints.append((var1, var2, professor_unico_por_horario))

cutoff_time = 20.0
max_restarts = 5
scheduler = SchedulerCSP(variables, domains, constraints, max_restarts, cutoff=cutoff_time)


solution = scheduler.backtrack(start_time)
end_time = time.perf_counter()



if solution is None:
    print("\n>>> Solução não encontrada. Possíveis causas:")
    print("1. Restrições conflitantes.")
    print("2. Domínios muito restritivos.")
    print("3. Não há salas ou horários suficientes.")

else:
    print("\n>>> SOLUÇÃO ENCONTRADA:")
    # Inicializa a estrutura para armazenar os horários por turma e dia da semana
    final_schedule = {turma: {dia: [] for dia in dias_da_semana} for turma in classes}

    # Organiza as aulas por turma e dia da semana
    for turma in classes:
        for aula in classes[turma]:
            if aula in solution:
                sala, prof, horario, dia = solution[aula]  # Ajustado para 4 elementos
                final_schedule[turma][dia].append((horario, aula, prof, sala))  # Armazena no formato desejado

    # Exibe o resultado final agrupado por turma e dia
    for turma, dias in final_schedule.items():
        print(f"{turma}:")
        for dia, aulas in dias.items():
            print(f"  {dia}: {aulas}")
        print("")



    print(f"Tempo de execução: {(end_time-start_time):.4f}s.\n")

