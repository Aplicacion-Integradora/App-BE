import { Component } from '@angular/core';
import { SerieDto } from '@proxy/series';

@Component({
  selector: 'app-series',
  templateUrl: './series.component.html',
  styleUrl: './series.component.scss'
})
export class SeriesComponent {
  series = [] as SerieDto[];
}
