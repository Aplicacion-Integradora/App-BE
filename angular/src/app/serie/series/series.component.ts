import { Component, OnInit } from '@angular/core';
// Importamos PagedAndSortedResultRequestDto para poder pedir la lista a ABP
import { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { SerieDto, SerieService } from '@proxy/series';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrls: ['./series.component.scss']
})
export class SeriesComponent implements OnInit {

  // Esta lista almacenará tanto resultados de BD como de OMDB
  series: any[] = []; 

  serieTitle: string = "";
  modoBusqueda: boolean = false; // Para saber si estamos viendo BD o API

  constructor(private serieService: SerieService) {}

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
      this.serieService.search(this.serieTitle.trim(), "")
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
  public importarSerie(titulo: string) {
    this.serieService.importarSerie(titulo).subscribe(() => {
        // Opcional: Mostrar una notificación de éxito aquí
        alert(`¡${titulo} guardada en tu colección!`);
        // Opcional: Limpiar búsqueda y volver a la lista
        this.serieTitle = "";
        this.cargarMisSeriesBD();
    });
  }
}