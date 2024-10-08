using JobsViewer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobsViewer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysJobsController : ControllerBase
    {

        private readonly JobsViewerContext _primaryConnectionString;
        private readonly IConfiguration _exceptionsJobs;
        private readonly IWebHostEnvironment _environment;
        private readonly DecryptConnectionString _decryptConnectionString;
        private static readonly HttpClient client = new HttpClient();

        public SysJobsController(IConfiguration configuration, IWebHostEnvironment environment, JobsViewerContext primaryDbContext, DecryptConnectionString decryptConnection)
        {
            _primaryConnectionString = primaryDbContext;
            _exceptionsJobs = configuration;
            _environment = environment;
            _decryptConnectionString = decryptConnection;

        }

        

        // GET: api/<SysJobsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SysJob>>> GetSysJobs()
        {
            try
            {
                // Obtener todas las cadenas de conexión de la sección "ConnectionStrings"
                var connectionStringsSection = _exceptionsJobs.GetSection("ConnectionStrings");
                var connectionStrings = connectionStringsSection.GetChildren()
                                              .Select(x => x.Value)
                                              .ToArray();

                var sysjobsfinal = new List<SysJob>();

                string rutaarchivoo = _exceptionsJobs["FileExecptionJobs:File"];

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
                    var sysjobs = new List<SysJob>();
                    // Desencriptar la cadena de conexión
                    decryptedConnectionString = _decryptConnectionString.DecryptConnectionAsync(connectionString).Result;
                    using (var connection = new SqlConnection(decryptedConnectionString))
                    {
                        await connection.OpenAsync();

                        var query = "SELECT distinct sj.Job_Id,Name, sj.Enabled, sj.Date_Created, sj.Date_Modified, ISNULL(sjh.server,'NA')  FROM  dbo.sysjobs as sj with(nolock) LEFT join dbo.sysjobhistory as sjh on sj.job_id = sjh.job_id where sj.Enabled = '1' and sj.Name NOT IN (" + String.Join(',', exceptions) + ")"; 
                        using (var command = new SqlCommand(query, connection))
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                sysjobs.Add(new SysJob
                                {
                                    Job_Id = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Enabled = reader.GetByte(2),
                                    Date_Created = reader.GetDateTime(3),
                                    Date_Modified = reader.GetDateTime(4),
                                    Server = reader.GetString(5)
                                });
                            }
                        }
                        sysjobsfinal.AddRange(sysjobs);
                    }
                }

                return Ok(sysjobsfinal);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        // GET api/<SysJobsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SysJob>> GetSysJobById(Guid id)
        {
            try
            {

                // Obtener todas las cadenas de conexión de la sección "ConnectionStrings"
                var connectionStringsSection = _exceptionsJobs.GetSection("ConnectionStrings");
                var connectionStrings = connectionStringsSection.GetChildren()
                                              .Select(x => x.Value)
                                              .ToArray();

                SysJob sysjobfinal = null;
                string decryptedConnectionString = string.Empty;

                foreach (var connectionString in connectionStrings)
                {
                    // Desencriptar la cadena de conexión
                    decryptedConnectionString = _decryptConnectionString.DecryptConnectionAsync(connectionString).Result;
                    Console.WriteLine("Decrypted Connection String: " + decryptedConnectionString); // Log the connection string
                    using (var connection = new SqlConnection(decryptedConnectionString))
                    {
                        SysJob sysjob = null;
                        await connection.OpenAsync();
                        var query = "SELECT Job_Id,Name, Enabled, Date_Created, Date_Modified  FROM dbo.sysjobs with(nolock) WHERE Job_Id = @Id";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    sysjob = new SysJob
                                    {
                                        Job_Id = reader.GetGuid(0),
                                        Name = reader.GetString(1),
                                        Enabled = reader.GetByte(2),
                                        Date_Created = reader.GetDateTime(3),
                                        Date_Modified = reader.GetDateTime(4)
                                    };
                                }
                            }
                            if (sysjob != null)
                            {
                                return Ok(sysjob);
                            }
                            else {
                                sysjobfinal = sysjob;
                            }
                        }
                    }
                }

                if (sysjobfinal == null)
                {
                    return NotFound();
                }

                return Ok(sysjobfinal);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
