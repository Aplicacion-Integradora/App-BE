import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

// Usamos los proxys generados para obtener los servicios de API Monitoring y Error Logs
import { ApiMonitoringService } from '@proxy/monitoring-logs';
import { ErrorLogService } from '@proxy/error-logs';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  apiLogs: any[] = [];
  errorLogs: any[] = [];
  cargando = true;

  stats = {
    totalPeticiones: 0,
    tiempoPromedio: 0,
    totalErrores: 0
  };

  constructor(
    private apiService: ApiMonitoringService, // Inyectamos el servicio automático
    private errorLogService: ErrorLogService  // Inyectamos el servicio automático
  ) {}

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos() {
    this.cargando = true;

    // Usamos forkJoin para pedir todo junto
    forkJoin({
      apis: this.apiService.getApiLogs(),       // Método del proxy
      errores: this.errorLogService.getErrorLogs() // Método del proxy
    }).subscribe({
      next: (res) => {
        this.apiLogs = res.apis;
        this.errorLogs = res.errores;
        this.calcularEstadisticas();
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error:', err);
        this.cargando = false;
      }
    });
  }

  calcularEstadisticas() {
    this.stats.totalPeticiones = this.apiLogs.length;
    this.stats.totalErrores = this.errorLogs.length;

    if (this.apiLogs.length > 0) {
      const suma = this.apiLogs.reduce((acc, log) => acc + (log.responseTime || 0), 0);
      this.stats.tiempoPromedio = Math.round(suma / this.apiLogs.length);
    }
  }
}