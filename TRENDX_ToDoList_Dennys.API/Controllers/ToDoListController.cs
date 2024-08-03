using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using TRENDX_ToDoList_Dennys.API.Entities;
using TRENDX_ToDoList_Dennys.API.Persistence;

namespace TRENDX_ToDoList_Dennys.API.Controllers
{
    [Route("api/to-do-list")]
    [ApiController]
    public class ToDoListController : ControllerBase
    {
        private readonly TarefaDbContext _tarefaDbContext;
        public ToDoListController(TarefaDbContext trefaDbContext)
        {
            _tarefaDbContext = trefaDbContext;
        }

        protected ActionResult CustomResponse(int tipoRetorno, object? result = null, string? message = null)
        {
            RetornoPadraoDto ret = new()
            {
                HasError = false,
                Data = result ?? new TarefaOutput(),
                Message = message
            };

            switch (tipoRetorno)
            {
                default:
                case StatusCodes.Status200OK:
                    return Ok(ret);
                case StatusCodes.Status400BadRequest:
                    ret.HasError = true;
                    return BadRequest(ret);
                case StatusCodes.Status404NotFound:
                    ret.Message = "Nenhuma tarefa encontrada.";
                    return NotFound(ret);
            }
        }

        private TarefaOutput RetornaTarefaOutput(Tarefa tarefa)
        {
            TarefaOutput tarefaOutput = new TarefaOutput();
            tarefaOutput.OutPut(tarefa);
            return tarefaOutput;
        }

        private string ValidarTarefa(TarefaInput tarefaInput)
        {
            string retorno = string.Empty;

            if (string.IsNullOrEmpty(tarefaInput.Title))
                retorno = retorno + (string.IsNullOrEmpty(retorno) ? "" : "\n") + "Title é um campo texto obrigatório.";

            if (tarefaInput.Title.Length > 50)
                retorno = retorno + (string.IsNullOrEmpty(retorno) ? "" : "\n") + "O campo Title possui um tamanho máximo de 50 caracteres.";

            if (string.IsNullOrEmpty(tarefaInput.Description))
                retorno = retorno + (string.IsNullOrEmpty(retorno) ? "" : "\n") + "Description é um campo texto obrigatório.";

            if (tarefaInput.Description.Length > 250)
                retorno = retorno + (string.IsNullOrEmpty(retorno) ? "" : "\n") + "O campo Description possui um tamanho máximo de 250 caracteres.";

            return retorno;
        }

        /// <summary>
        /// 1. Endpoint para Adicionar uma Tarefa:
        /// </summary>
        /// <remarks>
        /// Método HTTP: POST
        /// URL: /api/tasks
        /// Corpo da Requisição(JSON):
        /// JSON
        ///     {
        ///         "title": "Título da Tarefa",
        ///         "description": "Descrição da Tarefa",
        ///         "completed": false
        ///     }
        /// A API deve criar uma nova tarefa com os dados fornecidos e atribuir um ID único.
        /// </remarks>
        /// <param name="tarefaInput">Dados da Tarefa</param>
        /// <returns>Tarefa criada</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="400">ERRO</response>
        [HttpPost("adicionar-tarefa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AdicionarTarefa(TarefaInput tarefaInput)
        {
            string mensagemErro = ValidarTarefa(tarefaInput);
            if (!string.IsNullOrEmpty(mensagemErro))
                return CustomResponse(StatusCodes.Status400BadRequest, null, mensagemErro);

            Tarefa tarefa = new Tarefa();
            tarefa.Adicionar(tarefaInput);
            _tarefaDbContext.Tarefas.Add(tarefa);
            _tarefaDbContext.SaveChanges();

            return CustomResponse(StatusCodes.Status200OK, RetornaTarefaOutput(tarefa));
        }

        /// <summary>
        /// 2. Endpoint para Listar Todas as Tarefas:
        /// </summary>
        /// <remarks>
        ///    Método HTTP: GET
        ///    URL: /api/tasks
        ///    A API deve retornar uma lista de todas as tarefas existentes no formato JSON.
        /// </remarks>
        /// <returns>Coleção de Tarefas (TO DO LIST)</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet("obter-lista-tarefas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ListarTodasTarefas()
        {
            List<TarefaOutput> toDoList = new List<TarefaOutput>();
            foreach (var tarefa in _tarefaDbContext.Tarefas.Where(x => !x.IsDeleted))
                toDoList.Add(RetornaTarefaOutput(tarefa));

            return CustomResponse(StatusCodes.Status200OK, toDoList);
        }
        /// <summary>
        /// Complemento da atividade 2.
        /// </summary>
        /// <remarks>
        /// API retorna apenas 1 tarefa
        /// </remarks>
        /// <param name="id">Identificador da Tarefa</param>
        /// <returns>Retorna tarefa especifica conforme ID</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        [HttpGet("obter-tarefa-por-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult TarefaGetById(Guid id)
        {
            var tarefa = _tarefaDbContext.Tarefas.FirstOrDefault(x => !x.IsDeleted && x.Id == id);

            if (tarefa == null)
                return CustomResponse(StatusCodes.Status404NotFound);

            return CustomResponse(StatusCodes.Status200OK, RetornaTarefaOutput(tarefa));
        }

        /// <summary>
        /// 3. Endpoint para Atualizar uma Tarefa:
        /// </summary>
        /// <remarks>
        ///    Método HTTP: PUT
        ///    URL: /api/tasks/{id}
        ///    Parâmetro de Caminho(path parameter) : id é o ID da tarefa que deve ser atualizada.
        ///    Corpo da Requisição (JSON):
        ///    JSON
        ///        {
        ///            "title": "Novo Título",
        ///            "description": "Nova Descrição",
        ///            "completed": true
        ///        }
        ///    A API deve atualizar os dados da tarefa com o ID fornecido.
        /// </remarks>
        /// <param name="id">Identificador da Tarefa</param>
        /// <param name="tarefaInput">Dados da Tarefa</param>
        /// <returns>Tarefa Atualizada</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        [HttpPut("atualizar-tarefa/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizarTarefa(Guid id, TarefaInput tarefaInput)
        {
            var tarefa = _tarefaDbContext.Tarefas.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
            if (tarefa == null)
                return CustomResponse(StatusCodes.Status404NotFound);

            string mensagemErro = ValidarTarefa(tarefaInput);
            if (!string.IsNullOrEmpty(mensagemErro))
                return CustomResponse(StatusCodes.Status400BadRequest, null, mensagemErro);

            tarefa.Atualizar(tarefaInput.Title, tarefaInput.Description, tarefaInput.Completed);
            _tarefaDbContext.SaveChanges();
            return CustomResponse(StatusCodes.Status200OK, RetornaTarefaOutput(tarefa));
        }

        /// <summary>
        /// Complemento da atividade 3.
        /// </summary>
        /// <remarks>
        /// API Atualiza a tarefa para Concluido
        /// (Realizado a exclusão lógica.)
        /// </remarks>
        /// <param name="id">Identificador da Tarefa</param>
        /// <returns>Tarefa Atualizada</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        [HttpPut("concluir-tarefa/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ConcluirTarefa(Guid id)
        {
            var tarefa = _tarefaDbContext.Tarefas.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
            if (tarefa == null)
                return CustomResponse(StatusCodes.Status404NotFound);

            tarefa.Concluir();
            _tarefaDbContext.SaveChanges();
            return CustomResponse(StatusCodes.Status200OK, RetornaTarefaOutput(tarefa));
        }

        /// <summary>
        /// 4. Endpoint para Excluir uma Tarefa:
        /// </summary>
        /// <remarks>
        ///    Método HTTP: DELETE
        ///    URL: /api/tasks/{id}
        ///    Parâmetro de Caminho(path parameter) : id é o ID da tarefa que deve ser excluída.
        ///    A API deve excluir a tarefa com o ID fornecido.
        /// </remarks>
        /// <param name="id">Identificador da Tarefa</param>
        /// <returns>Tarefa Excluida</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        [HttpDelete("excluir-tarefa/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExcluirTarefa(Guid id)
        {
            var tarefa = _tarefaDbContext.Tarefas.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
            if (tarefa == null)
                return CustomResponse(StatusCodes.Status404NotFound);

            tarefa.Excluir();
            _tarefaDbContext.SaveChanges();
            return CustomResponse(StatusCodes.Status200OK, RetornaTarefaOutput(tarefa));
        }

        /// <summary>
        /// Complemento da atividade 4.
        /// </summary>
        /// <remarks>
        ///    Realizando a exclusão fisica.
        /// </remarks>
        /// <param name="id">Identificador da Tarefa</param>
        /// <returns>Tarefa Excluida</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        [HttpDelete("excluir-tarefa-db/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExcluirTarefaDb(Guid id)
        {
            var tarefa = _tarefaDbContext.Tarefas.FirstOrDefault(x => x.Id == id);
            if (tarefa == null)
                return CustomResponse(StatusCodes.Status404NotFound);
            _tarefaDbContext.Tarefas.Remove(tarefa);
            _tarefaDbContext.SaveChanges();
            return CustomResponse(StatusCodes.Status200OK, RetornaTarefaOutput(tarefa));
        }
    }
}
