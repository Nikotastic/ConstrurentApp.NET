import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TokenService } from '@application/services/token.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <!-- Welcome Banner -->
    <div
      class="bg-gradient-to-r from-blue-600 to-blue-800 rounded-xl shadow-lg p-6 mb-8 text-white relative overflow-hidden"
    >
      <div class="relative z-10">
        <h1 class="text-3xl font-bold mb-2">
          ¡Bienvenido de nuevo, {{ userName }}!
        </h1>
        <p class="text-blue-100 max-w-2xl">
          ¿Listo para tu próximo proyecto? Explora nuestro catálogo de
          maquinaria pesada y materiales de construcción.
        </p>
        <button
          routerLink="/products"
          class="mt-6 bg-orange-500 hover:bg-orange-600 text-white font-bold py-2 px-6 rounded-lg transition-colors shadow-md transform hover:-translate-y-1"
        >
          Ver Catálogo
        </button>
      </div>
      <div
        class="absolute right-0 bottom-0 opacity-10 transform translate-x-10 translate-y-10"
      >
        <svg width="300" height="300" fill="currentColor" viewBox="0 0 24 24">
          <path
            d="M12 0C5.373 0 0 5.373 0 12s5.373 12 12 12 12-5.373 12-12S18.627 0 12 0zm0 22c-5.523 0-10-4.477-10-10S6.477 2 12 2s10 4.477 10 10-4.477 10-10 10z"
          />
        </svg>
      </div>
    </div>

    <!-- Quick Actions Grid -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <div
        class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 hover:shadow-md transition-all duration-300 cursor-pointer group"
        routerLink="/products"
      >
        <div
          class="h-12 w-12 bg-orange-100 text-orange-600 rounded-lg flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
        >
          <span class="text-2xl">🏗️</span>
        </div>
        <h3 class="text-lg font-semibold text-gray-800 mb-1">
          Alquilar Maquinaria
        </h3>
        <p class="text-sm text-gray-500">
          Explora nuestra flota de equipos pesados.
        </p>
      </div>

      <div
        class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 hover:shadow-md transition-all duration-300 cursor-pointer group"
        routerLink="/products"
      >
        <div
          class="h-12 w-12 bg-blue-100 text-blue-600 rounded-lg flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
        >
          <span class="text-2xl">🧱</span>
        </div>
        <h3 class="text-lg font-semibold text-gray-800 mb-1">
          Comprar Materiales
        </h3>
        <p class="text-sm text-gray-500">
          Cemento, acero y agregados de calidad.
        </p>
      </div>

      <div
        class="bg-white p-6 rounded-xl shadow-sm border border-gray-100 hover:shadow-md transition-all duration-300 cursor-pointer group"
        routerLink="/orders"
      >
        <div
          class="h-12 w-12 bg-green-100 text-green-600 rounded-lg flex items-center justify-center mb-4 group-hover:scale-110 transition-transform"
        >
          <span class="text-2xl">📋</span>
        </div>
        <h3 class="text-lg font-semibold text-gray-800 mb-1">
          Historial de Pedidos
        </h3>
        <p class="text-sm text-gray-500">
          Revisa el estado de tus solicitudes.
        </p>
      </div>
    </div>

    <!-- Recent Activity (Placeholder) -->
    <div
      class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden"
    >
      <div
        class="px-6 py-4 border-b border-gray-100 flex justify-between items-center"
      >
        <h3 class="font-semibold text-gray-800">Actividad Reciente</h3>
        <button class="text-sm text-blue-600 hover:text-blue-800 font-medium">
          Ver todo
        </button>
      </div>
      <div class="p-6 text-center text-gray-500 py-12">
        <div class="text-4xl mb-3 opacity-50">📭</div>
        <p class="text-gray-400">No tienes actividad reciente.</p>
        <button
          routerLink="/products"
          class="mt-4 text-orange-500 hover:text-orange-600 font-medium hover:underline"
        >
          Comenzar a comprar →
        </button>
      </div>
    </div>
  `,
})
export class DashboardComponent implements OnInit {
  private tokenService = inject(TokenService);
  userName: string = 'Usuario';

  ngOnInit() {
    const name = this.tokenService.getUsername();
    if (name) {
      this.userName = name;
    }
  }
}
