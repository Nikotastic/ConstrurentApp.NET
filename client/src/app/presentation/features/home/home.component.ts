import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  standalone: true,
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule, RouterLink],
  template: `
    <div class="landing-page">
      <!-- Hero Section -->
      <section class="hero-section">
        <div class="hero-overlay"></div>
        <div class="hero-content">
          <div class="container">
            <div class="hero-text">
              <span class="badge-new">🚀 Plataforma #1 en Construcción</span>
              <h1 class="hero-title">
                Construye el Futuro con
                <span class="gradient-text">Firmness</span>
              </h1>
              <p class="hero-subtitle">
                Alquila maquinaria pesada y compra materiales de construcción de
                primera calidad. Todo en un solo lugar, rápido, seguro y al
                mejor precio.
              </p>
              <div class="hero-buttons">
                <a routerLink="/register" class="btn btn-primary btn-lg">
                  <svg
                    width="20"
                    height="20"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path
                      d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"
                    />
                  </svg>
                  Comenzar Ahora
                </a>
                <a routerLink="/login" class="btn btn-outline btn-lg">
                  <svg
                    width="20"
                    height="20"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path
                      fill-rule="evenodd"
                      d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z"
                      clip-rule="evenodd"
                    />
                  </svg>
                  Ver Demo
                </a>
              </div>
              <div class="hero-stats">
                <div class="stat">
                  <span class="stat-number">500+</span>
                  <span class="stat-label">Equipos Disponibles</span>
                </div>
                <div class="stat">
                  <span class="stat-number">1,200+</span>
                  <span class="stat-label">Clientes Satisfechos</span>
                </div>
                <div class="stat">
                  <span class="stat-number">98%</span>
                  <span class="stat-label">Tasa de Satisfacción</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="hero-image">
          <img
            src="https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=1200&q=80"
            alt="Construcción"
            class="floating-image"
          />
        </div>
      </section>

      <!-- ¿Qué es Firmness? -->
      <section class="about-section">
        <div class="container">
          <div class="section-header">
            <span class="section-badge">Sobre Nosotros</span>
            <h2 class="section-title">¿Qué es Firmness?</h2>
            <p class="section-subtitle">
              Somos la plataforma líder que conecta profesionales de la
              construcción con los mejores equipos y materiales
            </p>
          </div>
          <div class="about-grid">
            <div class="about-card">
              <div class="about-icon">🏗️</div>
              <h3>Nuestra Misión</h3>
              <p>
                Facilitar el acceso a maquinaria y materiales de construcción de
                alta calidad, haciendo que cada proyecto sea más eficiente y
                rentable.
              </p>
            </div>
            <div class="about-card">
              <div class="about-icon">🎯</div>
              <h3>Nuestra Visión</h3>
              <p>
                Ser la plataforma #1 en Latinoamérica para la industria de la
                construcción, transformando la manera en que se ejecutan los
                proyectos.
              </p>
            </div>
            <div class="about-card">
              <div class="about-icon">💎</div>
              <h3>Nuestros Valores</h3>
              <p>
                Confiabilidad, transparencia, innovación y compromiso con la
                excelencia en cada servicio que ofrecemos.
              </p>
            </div>
          </div>
        </div>
      </section>

      <!-- Servicios con Imágenes -->
      <section class="services-section">
        <div class="container">
          <div class="section-header">
            <span class="section-badge">Nuestros Servicios</span>
            <h2 class="section-title">Todo lo que Necesitas para Construir</h2>
          </div>
          <div class="services-grid">
            <div class="service-card">
              <div class="service-image">
                <img
                  src="https://images.unsplash.com/photo-1581094794329-c8112a89af12?w=600&q=80"
                  alt="Excavadora"
                />
                <div class="service-overlay">
                  <span class="service-badge">Más Popular</span>
                </div>
              </div>
              <div class="service-content">
                <h3>Alquiler de Maquinaria</h3>
                <p>
                  Excavadoras, grúas, retroexcavadoras y más. Equipos modernos
                  con mantenimiento garantizado.
                </p>
                <ul class="service-features">
                  <li>✓ Entrega en 24 horas</li>
                  <li>✓ Operadores certificados disponibles</li>
                  <li>✓ Seguro incluido</li>
                </ul>
                <a href="#" class="service-link">Ver Catálogo →</a>
              </div>
            </div>

            <div class="service-card">
              <div class="service-image">
                <img
                  src="https://images.unsplash.com/photo-1590856029826-c7a73142bbf1?w=600&q=80"
                  alt="Materiales"
                />
              </div>
              <div class="service-content">
                <h3>Venta de Materiales</h3>
                <p>
                  Cemento, acero, ladrillos, arena y todos los materiales que
                  necesitas para tu obra.
                </p>
                <ul class="service-features">
                  <li>✓ Precios mayoristas</li>
                  <li>✓ Marcas reconocidas</li>
                  <li>✓ Envío gratuito en compras grandes</li>
                </ul>
                <a href="#" class="service-link">Explorar Productos →</a>
              </div>
            </div>

            <div class="service-card">
              <div class="service-image">
                <img
                  src="https://images.unsplash.com/photo-1503387762-592deb58ef4e?w=600&q=80"
                  alt="Herramientas"
                />
              </div>
              <div class="service-content">
                <h3>Herramientas Especializadas</h3>
                <p>
                  Taladros, sierras, compresores y herramientas eléctricas de
                  última generación.
                </p>
                <ul class="service-features">
                  <li>✓ Alquiler por horas o días</li>
                  <li>✓ Equipos profesionales</li>
                  <li>✓ Asesoría técnica incluida</li>
                </ul>
                <a href="#" class="service-link">Ver Herramientas →</a>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Características Animadas -->
      <section class="features-section">
        <div class="container">
          <div class="features-grid">
            <div class="feature-item" *ngFor="let feature of features">
              <div
                class="feature-icon"
                [innerHTML]="getSafeHtml(feature.icon)"
              ></div>
              <h3>{{ feature.title }}</h3>
              <p>{{ feature.description }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Testimonios -->
      <section class="testimonials-section">
        <div class="container">
          <div class="section-header">
            <span class="section-badge">Testimonios</span>
            <h2 class="section-title">Lo que Dicen Nuestros Clientes</h2>
          </div>
          <div class="testimonials-grid">
            <div
              class="testimonial-card"
              *ngFor="let testimonial of testimonials"
            >
              <div class="testimonial-rating">
                <span *ngFor="let star of [1, 2, 3, 4, 5]">⭐</span>
              </div>
              <p class="testimonial-text">"{{ testimonial.text }}"</p>
              <div class="testimonial-author">
                <div class="author-avatar">{{ testimonial.initials }}</div>
                <div>
                  <div class="author-name">{{ testimonial.name }}</div>
                  <div class="author-role">{{ testimonial.role }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- FAQ -->
      <section class="faq-section">
        <div class="container">
          <div class="section-header">
            <span class="section-badge">Preguntas Frecuentes</span>
            <h2 class="section-title">¿Tienes Dudas?</h2>
          </div>
          <div class="faq-grid">
            <div
              class="faq-item"
              *ngFor="let faq of faqs; let i = index"
              (click)="toggleFaq(i)"
            >
              <div class="faq-question">
                <h3>{{ faq.question }}</h3>
                <span class="faq-icon" [class.active]="faq.open">+</span>
              </div>
              <div class="faq-answer" [class.open]="faq.open">
                <p>{{ faq.answer }}</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- CTA Final -->
      <section class="cta-section">
        <div class="container">
          <div class="cta-content">
            <h2>¿Listo para Comenzar tu Próximo Proyecto?</h2>
            <p>Únete a más de 1,200 profesionales que confían en Firmness</p>
            <div class="cta-buttons">
              <a routerLink="/register" class="btn btn-white btn-lg">
                Crear Cuenta Gratis
              </a>
              <a routerLink="/login" class="btn btn-outline-white btn-lg">
                Iniciar Sesión
              </a>
            </div>
          </div>
        </div>
      </section>

      <!-- Footer -->
      <footer class="footer">
        <div class="container">
          <div class="footer-grid">
            <div class="footer-col">
              <div class="footer-logo">
                <div class="logo-icon">F</div>
                <span>Firmness</span>
              </div>
              <p>
                Tu socio confiable en construcción. Conectamos profesionales con
                los mejores equipos y materiales.
              </p>
              <div class="social-links">
                <a href="#"
                  ><svg
                    width="24"
                    height="24"
                    fill="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"
                    /></svg
                ></a>
                <a href="#"
                  ><svg
                    width="24"
                    height="24"
                    fill="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M23.953 4.57a10 10 0 01-2.825.775 4.958 4.958 0 002.163-2.723c-.951.555-2.005.959-3.127 1.184a4.92 4.92 0 00-8.384 4.482C7.69 8.095 4.067 6.13 1.64 3.162a4.822 4.822 0 00-.666 2.475c0 1.71.87 3.213 2.188 4.096a4.904 4.904 0 01-2.228-.616v.06a4.923 4.923 0 003.946 4.827 4.996 4.996 0 01-2.212.085 4.936 4.936 0 004.604 3.417 9.867 9.867 0 01-6.102 2.105c-.39 0-.779-.023-1.17-.067a13.995 13.995 0 007.557 2.209c9.053 0 13.998-7.496 13.998-13.985 0-.21 0-.42-.015-.63A9.935 9.935 0 0024 4.59z"
                    /></svg
                ></a>
                <a href="#"
                  ><svg
                    width="24"
                    height="24"
                    fill="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M12 0C5.373 0 0 5.373 0 12s5.373 12 12 12 12-5.373 12-12S18.627 0 12 0zm4.441 16.892c-2.102.144-6.784.144-8.883 0C5.282 16.736 5.017 15.622 5 12c.017-3.629.285-4.736 2.558-4.892 2.099-.144 6.782-.144 8.883 0C18.718 7.264 18.982 8.378 19 12c-.018 3.629-.285 4.736-2.559 4.892zM10 9.658l4.917 2.338L10 14.342V9.658z"
                    /></svg
                ></a>
              </div>
            </div>
            <div class="footer-col">
              <h4>Servicios</h4>
              <ul>
                <li><a href="#">Alquiler de Maquinaria</a></li>
                <li><a href="#">Venta de Materiales</a></li>
                <li><a href="#">Herramientas</a></li>
                <li><a href="#">Asesoría Técnica</a></li>
              </ul>
            </div>
            <div class="footer-col">
              <h4>Empresa</h4>
              <ul>
                <li><a href="#">Sobre Nosotros</a></li>
                <li><a href="#">Contacto</a></li>
                <li><a href="#">Trabaja con Nosotros</a></li>
                <li><a href="#">Blog</a></li>
              </ul>
            </div>
            <div class="footer-col">
              <h4>Legal</h4>
              <ul>
                <li><a href="#">Términos y Condiciones</a></li>
                <li><a href="#">Política de Privacidad</a></li>
                <li><a href="#">Política de Cookies</a></li>
              </ul>
            </div>
          </div>
          <div class="footer-bottom">
            <p>&copy; 2025 Firmness. Todos los derechos reservados.</p>
          </div>
        </div>
      </footer>
    </div>
  `,
})
export class HomeComponent {
  constructor(private sanitizer: DomSanitizer) {}

  features = [
    {
      icon: '<svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M5 2a1 1 0 011 1v1h1a1 1 0 010 2H6v1a1 1 0 01-2 0V6H3a1 1 0 010-2h1V3a1 1 0 011-1zm0 10a1 1 0 011 1v1h1a1 1 0 110 2H6v1a1 1 0 11-2 0v-1H3a1 1 0 110-2h1v-1a1 1 0 011-1zM12 2a1 1 0 01.967.744L14.146 7.2 17.5 9.134a1 1 0 010 1.732l-3.354 1.935-1.18 4.455a1 1 0 01-1.933 0L9.854 12.8 6.5 10.866a1 1 0 010-1.732l3.354-1.935 1.18-4.455A1 1 0 0112 2z" clip-rule="evenodd"/></svg>',
      title: 'Equipos Certificados',
      description:
        'Toda nuestra maquinaria cuenta con certificaciones de seguridad y mantenimiento preventivo',
    },
    {
      icon: '<svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20"><path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z"/><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd"/></svg>',
      title: 'Precios Competitivos',
      description:
        'Los mejores precios del mercado sin comprometer la calidad de nuestros servicios',
    },
    {
      icon: '<svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"/></svg>',
      title: 'Disponibilidad 24/7',
      description:
        'Atención al cliente y soporte técnico disponible las 24 horas del día, todos los días',
    },
    {
      icon: '<svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M2.166 4.999A11.954 11.954 0 0010 1.944 11.954 11.954 0 0017.834 5c.11.65.166 1.32.166 2.001 0 5.225-3.34 9.67-8 11.317C5.34 16.67 2 12.225 2 7c0-.682.057-1.35.166-2.001zm11.541 3.708a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/></svg>',
      title: 'Seguro Incluido',
      description:
        'Todos nuestros alquileres incluyen seguro contra daños y responsabilidad civil',
    },
    {
      icon: '<svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20"><path d="M8 16.5a1.5 1.5 0 11-3 0 1.5 1.5 0 013 0zM15 16.5a1.5 1.5 0 11-3 0 1.5 1.5 0 013 0z"/><path d="M3 4a1 1 0 00-1 1v10a1 1 0 001 1h1.05a2.5 2.5 0 014.9 0H10a1 1 0 001-1V5a1 1 0 00-1-1H3zM14 7a1 1 0 00-1 1v6.05A2.5 2.5 0 0115.95 16H17a1 1 0 001-1v-5a1 1 0 00-.293-.707l-2-2A1 1 0 0015 7h-1z"/></svg>',
      title: 'Entrega Rápida',
      description:
        'Entregamos tus equipos y materiales en menos de 24 horas en toda la ciudad',
    },
    {
      icon: '<svg width="48" height="48" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-6-3a2 2 0 11-4 0 2 2 0 014 0zm-2 4a5 5 0 00-4.546 2.916A5.986 5.986 0 0010 16a5.986 5.986 0 004.546-2.084A5 5 0 0010 11z" clip-rule="evenodd"/></svg>',
      title: 'Asesoría Experta',
      description:
        'Nuestro equipo de expertos te ayuda a elegir el equipo perfecto para tu proyecto',
    },
  ];

  testimonials = [
    {
      text: 'Firmness transformó la manera en que gestionamos nuestros proyectos. El alquiler de maquinaria es rápido y confiable.',
      name: 'Carlos Méndez',
      role: 'Ingeniero Civil',
      initials: 'CM',
    },
    {
      text: 'Los precios son excelentes y la calidad de los materiales es superior. Definitivamente mi primera opción.',
      name: 'María González',
      role: 'Contratista',
      initials: 'MG',
    },
    {
      text: 'El servicio al cliente es excepcional. Siempre están disponibles para resolver cualquier duda.',
      name: 'Roberto Silva',
      role: 'Constructor',
      initials: 'RS',
    },
  ];

  faqs = [
    {
      question: '¿Cómo funciona el alquiler de maquinaria?',
      answer:
        'Simplemente selecciona el equipo que necesitas, elige las fechas de alquiler y realiza el pago. Nosotros nos encargamos de la entrega y recogida.',
      open: false,
    },
    {
      question: '¿Qué incluye el precio del alquiler?',
      answer:
        'El precio incluye el seguro, mantenimiento preventivo, entrega y recogida del equipo. También puedes contratar un operador certificado por un costo adicional.',
      open: false,
    },
    {
      question: '¿Hacen entregas fuera de la ciudad?',
      answer:
        'Sí, realizamos entregas a nivel nacional. Los costos de envío varían según la distancia y el tipo de equipo.',
      open: false,
    },
    {
      question: '¿Qué métodos de pago aceptan?',
      answer:
        'Aceptamos tarjetas de crédito, débito, transferencias bancarias y pagos en efectivo. También ofrecemos planes de financiamiento para proyectos grandes.',
      open: false,
    },
    {
      question: '¿Puedo cancelar o modificar mi reserva?',
      answer:
        'Sí, puedes cancelar o modificar tu reserva hasta 24 horas antes de la fecha de entrega sin ningún cargo adicional.',
      open: false,
    },
  ];

  toggleFaq(index: number): void {
    this.faqs[index].open = !this.faqs[index].open;
  }

  getSafeHtml(html: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}
