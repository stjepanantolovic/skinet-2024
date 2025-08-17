import { Component } from '@angular/core';
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

}
