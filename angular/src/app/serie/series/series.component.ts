import { Component, OnInit } from '@angular/core';
// Importamos PagedAndSortedResultRequestDto para poder pedir la lista a ABP
import { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { SerieDto, SerieService } from '@proxy/series';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrls: ['./series.component.scss']
})
export class SeriesComponent implements OnInit {

  // Esta lista almacenará tanto resultados de BD como de OMDB
  series: any[] = [];

  serieTitle: string = "";
  selectedGenre: string = "";

  genres: string[] = [
    "Action", "Comedy", "Drama", "Fantasy", "Horror", "Mystery", "Romance", "Thriller", "Western", "Sci-Fi", "Crime", "Animation", "Adventure"
  ];

  modoBusqueda: boolean = false; // Para saber si estamos viendo BD o API

  constructor(private serieService: SerieService, private modalService: NgbModal) { }

  // AL INICIAR: Carga la Operación 2.1 (Base de Datos)
  ngOnInit(): void {
    this.cargarMisSeriesBD();
  }

  // OPERACIÓN 2.1: Traer de la Base de Datos interna
  cargarMisSeriesBD() {
    const input = { maxResultCount: 100 } as PagedAndSortedResultRequestDto;

    this.serieService.getList(input).subscribe((response) => {
      this.series = response.items;
      this.modoBusqueda = false; // Estamos en modo "Mi Biblioteca"
    });
  }

  // OPERACIÓN 1.1: Buscar en API Externa
  public searchSeries() {
    if (this.serieTitle.trim()) {
      this.serieService.search(this.serieTitle.trim(), this.selectedGenre)
        .subscribe(response => {
          this.series = response || [];
          this.modoBusqueda = true; // Estamos en modo "Buscando fuera"
        });
    } else {
      // Si el usuario limpia el buscador, volvemos a mostrar la BD
      this.cargarMisSeriesBD();
    }
  }

  // OPERACIÓN 2.2: Guardar (Importar de API a BD)
  public importarSerie(imdbId: string, title: string) {
    this.serieService.importarSerie(imdbId).subscribe(() => {
      // Opcional: Mostrar una notificación de éxito aquí
      alert(`¡${title} guardada en tu colección!`);
      // Opcional: Limpiar búsqueda y volver a la lista
      this.serieTitle = "";
      this.cargarMisSeriesBD();
    });
  }

  // OPERACIÓN: Ver Detalles
  selectedSerie: SerieDto = {} as SerieDto;

  openDetails(content: any, serieId: number) {
    this.serieService.get(serieId).subscribe(serie => {
      this.selectedSerie = serie;
      this.modalService.open(content, { size: 'lg', scrollable: true });
    });
  }
}