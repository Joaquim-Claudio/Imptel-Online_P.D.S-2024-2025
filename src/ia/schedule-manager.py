
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

#https://pypi.org/project/constraint/ -> tem esta biblioteca mas para nota final valoriza-se a implementação de algoritmos.

from itertools import product

# Representação das variáveis e domínios
turmas = ["TurmaA", "TurmaB"]
disciplinas = ["Matemática", "Português"]
professores = {"Matemática": "Prof1", "Português": "Prof2"}
salas = {"Sala1": 30, "Sala2": 25}  # Capacidade das salas
horarios = ["9h00", "10h00", "11h00"]
dominio = list(product(salas.keys(), horarios))  # Combinação de salas e horários

# Inicializar as variáveis com os seus domínios
variaveis = {f"{disciplina}_{turma}": dominio for turma in turmas for disciplina in disciplinas}
print(variaveis)

# Função para verificar se uma alocação é válida
def verifica_restricoes(alocacao, variavel, valor):
    sala, horario = valor
    disciplina, turma = variavel.split("_")
    professor = professores[disciplina]

    # Restrições: um professor por horário
    for var, val in alocacao.items():
        if val[1] == horario and professores[var.split("_")[0]] == professor:
            return False
        # Restrições: uma turma por horário
        if val[1] == horario and var.split("_")[1] == turma:
            return False
        # Restrições: capacidade da sala
        if salas[sala] < 30:  # Exemplo: turmas com pelo menos 30 alunos
            return False
    return True

# Backtracking para resolver o problema
def backtracking(alocacao):
    if len(alocacao) == len(variaveis):
        return alocacao  # Todas as variáveis estão atribuídas

    # Selecionar a próxima variável
    variavel = next(var for var in variaveis if var not in alocacao)

    for valor in variaveis[variavel]:
        if verifica_restricoes(alocacao, variavel, valor):
            alocacao[variavel] = valor
            resultado = backtracking(alocacao)
            if resultado:
                return resultado
            del alocacao[variavel]  # Retroceder

    return None

# Resolver o CSP
solucao = backtracking({})
print("Solução encontrada:", solucao)

