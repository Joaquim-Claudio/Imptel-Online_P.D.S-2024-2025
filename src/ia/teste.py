class GestorAlocacao:
    def __init__(self, turmas, horarios_disponiveis, capacidade_salas):
        self.turmas = turmas
        self.horarios_disponiveis = horarios_disponiveis
        self.capacidade_salas = capacidade_salas
        self.alocacao = {}  

    def verificar_capacidade(self, sala, num_alunos):
        return self.capacidade_salas[sala] >= num_alunos

    def verificar_disponibilidade(self, horario, sala):
        for turma, aloc in self.alocacao.items():
            if aloc == (horario, sala):
                return False
        return True

    def tentar_alocar_turma(self, turma, num_alunos, disciplina):
        print(f"Tentando alocar turma '{turma}' ({disciplina}) com {num_alunos} alunos.")
        for horario, sala in self.horarios_disponiveis[disciplina]:
            if self.verificar_capacidade(sala, num_alunos) and self.verificar_disponibilidade(horario, sala):
                print(f"Alocando {horario}, {sala} para turma '{turma}' ({disciplina})")
                self.alocacao[turma] = (horario, sala)
                return True
            else:
                if not self.verificar_capacidade(sala, num_alunos):
                    print(f"Sala {sala} não suporta {num_alunos} alunos.")
                if not self.verificar_disponibilidade(horario, sala):
                    print(f"Sala {sala} já ocupada em {horario}.")
        print(f"Falha ao alocar turma '{turma}' ({disciplina})")
        return False

    def desalocar_turma(self, turma):
        if turma in self.alocacao:
            horario, sala = self.alocacao[turma]
            print(f"Desfazendo alocação de {horario}, {sala} para turma '{turma}'")
            del self.alocacao[turma]

    def backtracking(self):
        for turma, (disciplina, num_alunos) in self.turmas.items():
            if not self.tentar_alocar_turma(turma, num_alunos, disciplina):
                print(f"Tentando realocação para resolver conflitos da turma '{turma}'")
                if not self.relocate_turmas(turma, disciplina, num_alunos):
                    print(f"Todas as tentativas falharam para a turma '{turma}'")
                    return False
        return True

    def relocate_turmas(self, turma_falha, disciplina, num_alunos):
        """Tenta realocar turmas já alocadas para abrir espaço."""
        for turma, (horario, sala) in list(self.alocacao.items()):
            # Verifique se a realocação da turma original libera espaço para a turma_falha
            if self.tentar_alocar_turma(turma_falha, num_alunos, disciplina):
                print(f"Realocando '{turma_falha}' para permitir alocação")
                return True
        print(f"Impossível realocar '{turma_falha}' devido a limitações de capacidade e horários")
        return False

    def imprimir_solucao(self):
        if self.backtracking():
            print("Alocação concluída com sucesso:")
            for turma, (horario, sala) in self.alocacao.items():
                print(f"Turma '{turma}' alocada em {horario}, {sala}")
        else:
            print("Não foi possível encontrar uma solução válida.")

# Exemplo de uso:
turmas = {
    'turma1': ('matematica', 25),
    'turma2': ('matematica', 35),
    'turma3': ('fisica', 20),
}
horarios_disponiveis = {
    'matematica': [('segunda', 'S1'), ('terça', 'S2')],
    'fisica': [('quarta', 'S3')]
}
capacidade_salas = {'S1': 50, 'S2': 50, 'S3': 50}

gestor = GestorAlocacao(turmas, horarios_disponiveis, capacidade_salas)
gestor.imprimir_solucao()

