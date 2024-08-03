namespace TRENDX_ToDoList_Dennys.API.Entities
{
    public class Tarefa
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public bool IsDeleted { get; set; }

        public void Adicionar(TarefaInput tarefaInput)
        {
            Id = Guid.NewGuid();
            Title = tarefaInput.Title;
            Description = tarefaInput.Description;
            Completed = tarefaInput.Completed;
            IsDeleted = false;
        }

        public void Atualizar(string title, string description, bool completed)
        {
            Title = title;
            Description = description;
            Completed = completed;
        }

        public void Excluir()
        {
            IsDeleted = true;
        }

        public void Concluir()
        {
            Completed = true;
        }
    }

    public class TarefaInput
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }

    public class TarefaOutput
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }

        public void OutPut(Tarefa tarefa)
        {
            Id = tarefa.Id;
            Title = tarefa.Title;
            Description = tarefa.Description;
            Completed = tarefa.Completed;
        }
    }
}
