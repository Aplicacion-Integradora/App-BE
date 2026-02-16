import { Component, OnInit } from '@angular/core';
import { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { SerieDto, SerieService } from '@proxy/series';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// 👇 1. IMPORTANTE: Importamos SweetAlert2
import Swal from 'sweetalert2';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrls: ['./series.component.scss']
})
export class SeriesComponent implements OnInit {

  series: any[] = [];
  savedImdbIds: Set<string> = new Set(); 
  serieTitle: string = "";
  selectedGenre: string = "";
  haBuscado: boolean = false; 
  modoBusqueda: boolean = false;
  
  genres: string[] = ["Action", "Comedy", "Drama", "Fantasy", "Horror", "Mystery", "Romance", "Thriller", "Sci-Fi"];
  selectedSerie: SerieDto = {} as SerieDto;

  constructor(private serieService: SerieService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.cargarMisSeriesBD();
  }

  cargarMisSeriesBD() {
    const input = { maxResultCount: 100 } as PagedAndSortedResultRequestDto;
    this.serieService.getList(input).subscribe((response) => {
      this.series = response.items;
      this.modoBusqueda = false;
      this.haBuscado = false;

      this.savedImdbIds.clear();
      this.series.forEach(s => {
        // Validación extra por si imdbId viene null de la BD antigua
        const id = s.imdbID || s.imdbId;
        if (id) this.savedImdbIds.add(id);
      });
    });
  }

  public searchSeries() {
    if (!this.serieTitle.trim()) {
      if(this.modoBusqueda) this.cargarMisSeriesBD();
      return;
    }
    this.haBuscado = true;
    this.serieService.search(this.serieTitle.trim(), this.selectedGenre)
      .subscribe(response => {
        this.series = response || [];
        this.modoBusqueda = true;
      });
  }

  isAlreadySaved(imdbId: string): boolean {
    if (!imdbId) return false;
    return this.savedImdbIds.has(imdbId);
  }

  // 👇 2. AQUÍ ESTÁ LA MAGIA VISUAL
  public importarSerie(imdbId: string, title: string) {
    
    // Validación silenciosa (para no molestar al usuario si hay un error técnico)
    if (!imdbId) {
      console.error("Error: ID inválido");
      return;
    }

    // Si ya existe, mostramos un aviso lindo
    if (this.isAlreadySaved(imdbId)) {
      Swal.fire({
        title: '¡Ya la tienes!',
        text: `"${title}" ya está en tu biblioteca.`,
        icon: 'info',
        confirmButtonColor: '#333', // Gris oscuro
        confirmButtonText: 'Entendido',
        background: '#141414', // Fondo oscuro (tema Netflix)
        color: '#fff' // Texto blanco
      });
      return;
    }

    // Intentamos guardar
    this.serieService.importarSerie(imdbId).subscribe({
      next: () => {
        this.savedImdbIds.add(imdbId);
        
        // ✅ ÉXITO: Cartel Estético
        Swal.fire({
          title: '¡Agregada!',
          text: `"${title}" se ha guardado correctamente.`,
          icon: 'success',
          confirmButtonColor: '#e50914', // Rojo Netflix
          confirmButtonText: 'Genial',
          background: '#141414',
          color: '#fff',
          timer: 5000, // Se cierra solo a los 3 segundos
          timerProgressBar: true
        });
      },
      error: (err) => {
        console.error(err);
        // ❌ ERROR: Cartel Estético
        Swal.fire({
          title: 'Error',
          text: 'No se pudo guardar la serie. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonColor: '#e50914',
          background: '#141414',
          color: '#fff'
        });
      }
    });
  }

  openDetails(content: any, item: any) {
    
    // CASO 1: Estamos buscando en la API (OMDB)
    if (this.modoBusqueda) {
      // Mapeamos los datos básicos de OMDB al objeto que usa el modal
      this.selectedSerie = {
        title: item.Title,
        releaseDate: item.Year,
        image: item.Poster !== 'N/A' ? item.Poster : null,
        description: 'Descripción completa disponible tras agregar a la biblioteca.', // Placeholder
        rating: 'N/A',
        genre: 'Desconocido', // La búsqueda simple no trae género
        seasons: [] 
      } as any;

      // Abrimos el modal directamente
      this.modalService.open(content, { size: 'lg', scrollable: true, windowClass: 'dark-modal' });
    } 
    
    // CASO 2: Es una serie guardada en BD (Traemos todo el detalle)
    else {
      this.serieService.get(item.id).subscribe(serie => {
        this.selectedSerie = serie;
        this.modalService.open(content, { size: 'lg', scrollable: true, windowClass: 'dark-modal' });
      });
    }
  }
}