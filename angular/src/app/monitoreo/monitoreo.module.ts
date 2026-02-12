import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonitoreoRoutingModule } from './monitoreo-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';

@NgModule({
  declarations: [DashboardComponent],
  imports: [
    CommonModule,
    MonitoreoRoutingModule // 👈 Importante: Esto conecta las rutas
  ]
})
export class MonitoreoModule { }
