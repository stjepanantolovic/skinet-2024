import { Component, DOCUMENT, inject, Renderer2 } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HomeCarouselComponent } from "./home-carousel/home-carousel.component";

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
    HomeCarouselComponent
],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
   private renderer = inject(Renderer2);
  private doc = inject(DOCUMENT);

  ngOnInit() {
    this.renderer.addClass(this.doc.body, 'home-gradient');
  }

  ngOnDestroy() {
    this.renderer.removeClass(this.doc.body, 'home-gradient');
  }

}
