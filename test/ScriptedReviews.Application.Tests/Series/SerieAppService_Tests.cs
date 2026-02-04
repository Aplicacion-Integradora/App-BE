using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;
using ScriptedReviews;

namespace ScriptedReviews.Series
{
    // Heredamos de ...ApplicationTestBase para tener acceso a la BD en memoria y servicios
    public class SerieAppService_Tests : ScriptedReviewsApplicationTestBase<ScriptedReviewsApplicationTestModule>
    {
        private readonly ISerieAppService _serieAppService;
        private readonly IRepository<Serie, int> _serieRepository;

        public SerieAppService_Tests()
        {
            // Inyectamos el servicio que queremos probar y el repositorio para verificar
            _serieAppService = GetRequiredService<ISerieAppService>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
        }

        [Fact]
        public async Task ImportarSerieAsync_Deberia_Traer_Serie_De_Omdb_Y_Guardarla()
        {
            // 1. ARRANG (Preparación)
            var tituloBusqueda = "Breaking Bad";

            // 2. ACT (Acción)
            // El servicio hace su trabajo, guarda y cierra la conexión.
            var resultado = await _serieAppService.ImportarSerieAsync(tituloBusqueda);

            // 3. ASSERT (Verificación)
            resultado.ShouldNotBeNull();
            resultado.Title.ShouldBe("Breaking Bad");
            resultado.ImdbId.ShouldNotBeNullOrEmpty();

            // Abrimos un nuevo "UnitOfWork" (conexión) solo para consultar la BD
            await WithUnitOfWorkAsync(async () =>
            {
                var serieEnDb = await _serieRepository.FirstOrDefaultAsync(x => x.ImdbId == resultado.ImdbId);
                serieEnDb.ShouldNotBeNull(); // ¡Ahora sí encontrará la caja abierta!
                serieEnDb.Title.ShouldBe("Breaking Bad");
            });
        }

        [Fact]
        public async Task ImportarSerieAsync_No_Deberia_Duplicar_Si_Ya_Existe()
        {
            // 1. Preparación: Insertamos la serie una vez
            await _serieAppService.ImportarSerieAsync("Game of Thrones");

            // Contamos cuántas hay (Debería haber 1)
            var cantidadInicial = await _serieRepository.GetCountAsync();

            // 2. Acción: Intentamos importarla de nuevo
            var resultadoRepetido = await _serieAppService.ImportarSerieAsync("Game of Thrones");

            // 3. Verificación
            var cantidadFinal = await _serieRepository.GetCountAsync();

            // La cantidad debe ser la misma, no debió crear otra fila
            cantidadFinal.ShouldBe(cantidadInicial);

            // Y el objeto devuelto debe ser el mismo (mismo ID)
            resultadoRepetido.Title.ShouldBe("Game of Thrones");
        }
    }
}