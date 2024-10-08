using JobsViewer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobsViewer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysJobHistoryController : ControllerBase
    {
        private readonly JobsViewerContext _primaryConnectionString;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly DecryptConnectionString _decryptConnectionString;

        public SysJobHistoryController(IConfiguration configuration, IWebHostEnvironment environment, JobsViewerContext primaryDbContext, DecryptConnectionString decryptConnection)
        {
            _primaryConnectionString = primaryDbContext;
            _configuration = configuration;
            _environment = environment;
            _decryptConnectionString = decryptConnection;
        }

        // GET: api/<SysJobHistoryController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysJobHistory>>> GetSysJobHistory()
        {
            try
            {
                var connectionStringsSection = _configuration.GetSection("ConnectionStrings");
                var connectionStrings = connectionStringsSection.GetChildren()
                                              .Select(x => x.Value)
                                              .ToArray();

                var sysjobhistoryfinal = new List<SysJobHistory>();

                //Buscar archivos de excepciones
                string rutaarchivoo = _configuration["FileExecptionJobs:File"];
                string rutaArchivo = Path.Combine(_environment.WebRootPath, rutaarchivoo);

                if (!System.IO.File.Exists(rutaArchivo))
                {
                    return NotFound("El archivo no existe.");
                }


                string contenido;
                using (StreamReader reader = new StreamReader(rutaArchivo))
                {
                    contenido = await reader.ReadToEndAsync();
                }

                string[] exceptions = contenido.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);

                string decryptedConnectionString = string.Empty;

                foreach (var connectionString in connectionStrings)
                {
                    var sysjobhistory = new List<SysJobHistory>();
                    // Desencriptar la cadena de conexión
                    decryptedConnectionString = _decryptConnectionString.DecryptConnectionAsync(connectionString).Result;
                    using (var connection = new SqlConnection(decryptedConnectionString))
                    {
                        await connection.OpenAsync();

                        var query = "SELECT sjh.instance_id, sjh.run_date, sjh.run_time, sj.name, ISNULL(sjh.job_id,'N/A'), sjh.message, sjh.run_status, sjh.run_duration, sjh.server, sjh.step_name  FROM [msdb].[dbo].[sysjobhistory]  as sjh with (nolock) LEFT JOIN dbo.sysjobs as sj ON sj.job_id = sjh.job_id where sjh.run_status = '0'" + "and sj.Name NOT IN(" + String.Join(',', exceptions) + ") ORDER BY sjh.run_date desc, sjh.run_time desc";
                        using (var command = new SqlCommand(query, connection))
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                sysjobhistory.Add(new SysJobHistory
                                {
                                    instance_Id = reader.GetInt32(0),
                                    Run_Date = reader.GetInt32(1),
                                    Run_Time = reader.GetInt32(2),
                                    job_name = reader.GetString(3),
                                    job_id = reader.GetGuid(4),
                                    Message = reader.GetString(5),
                                    Run_Status = reader.GetInt32(6),
                                    Run_Duration = reader.GetInt32(7),
                                    Server = reader.GetString(8),
                                    Step_Name = reader.GetString(9)
                                });
                            }
                        }
                        sysjobhistoryfinal.AddRange(sysjobhistory);
                    }
                }
                return Ok(sysjobhistoryfinal);
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET api/<SysJobHistoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SysJobHistory>>> GetSysJobHistoryById(Guid id)
        {
            try
            {
                var connectionStringsSection = _configuration.GetSection("ConnectionStrings");
                var connectionStrings = connectionStringsSection.GetChildren()
                                              .Select(x => x.Value)
                                              .ToArray();

                var sysjobhistoryfinal = new List<SysJobHistory>();
                string decryptedConnectionString = string.Empty;

                foreach (var connectionsString in connectionStrings)
                {
                    var sysjobhistory = new List<SysJobHistory>();
                    // Desencriptar la cadena de conexión
                    decryptedConnectionString = _decryptConnectionString.DecryptConnectionAsync(connectionsString).Result;
                    using (var connection = new SqlConnection(decryptedConnectionString))
                    {
                        await connection.OpenAsync();
                        var query = "SELECT sjh.instance_id, sjh.run_date, sjh.run_time, sj.name, sjh.job_id, sjh.message, sjh.run_status, sjh.run_duration, ISNULL(sjh.server,'N/A'), sjh.step_name  FROM [msdb].[dbo].[sysjobhistory]  as sjh with (nolock) LEFT JOIN dbo.sysjobs as sj ON sj.job_id = sjh.job_id WHERE sjh.job_id = @Id order by run_date desc, run_time desc";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    sysjobhistory.Add(new SysJobHistory
                                    {
                                        instance_Id = reader.GetInt32(0),
                                        Run_Date = reader.GetInt32(1),
                                        Run_Time = reader.GetInt32(2),
                                        job_name = reader.GetString(3),
                                        job_id = reader.GetGuid(4),
                                        Message = reader.GetString(5),
                                        Run_Status = reader.GetInt32(6),
                                        Run_Duration = reader.GetInt32(7),
                                        Server = reader.GetString(8),
                                        Step_Name = reader.GetString(9)
                                    });
                                }
                            }
                            sysjobhistoryfinal.AddRange(sysjobhistory);
                        }
                    }
                }

                

                if (sysjobhistoryfinal.Count == 0)
                {
                    return NotFound();
                }

                return Ok(sysjobhistoryfinal);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
