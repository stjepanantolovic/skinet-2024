import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Carousel, CarouselModule } from 'primeng/carousel';
import { CardModule } from 'primeng/card';
import { RouterLink } from '@angular/router';

export interface CarouselItem {
  name: string;
  image: string;
  price?: number;
  badge?: string;
  description?: string;
}

@Component({
  selector: 'app-home-carousel',
  standalone: true,
  imports: [CommonModule, CarouselModule, CardModule, RouterLink],
  templateUrl: './home-carousel.component.html',
  styleUrls: ['./home-carousel.component.scss'],
})
export class HomeCarouselComponent implements OnInit, OnDestroy {
  @ViewChild('c') carousel?: Carousel;

  @Input() items: CarouselItem[] = [
    { name: 'Doors', image: 'https://res.cloudinary.com/dct9az8d1/image/upload/v1755461156/images/images/KSK402_20250817200554.jpg' },
    { name: 'Windows', image: 'https://res.cloudinary.com/dct9az8d1/image/upload/v1755468430/images/images/alustolarija_optimized_20250817220710.webp' },
    { name: 'Kitchens', image: 'https://res.cloudinary.com/dct9az8d1/image/upload/v1755458852/images/images/Whitekitchen-3_20250817192729.png' },
    { name: 'Furniture', image: 'https://res.cloudinary.com/dct9az8d1/image/upload/v1755461027/images/images/STV119-Room-2_20250817200344.png' }
    // { name: 'AI Designer', image: 'https://res.cloudinary.com/dct9az8d1/image/upload/v1755467011/images/images/AIDesignHomes_stretch_480x270_20250817214330.webp' },
  ];

  // Desktop defaults; smaller screens handled by responsiveOptions
  @Input() numVisible = 4;
  @Input() numScroll = 1;
  @Input() circular = true;
  @Input() showIndicators = true;
  @Input() showNavigators = true;

  // 2-up on phones, 3 on tablets/laptops, 4 on large screens
  responsiveOptions = [
    { breakpoint: '2000px', numVisible: 3, numScroll: 1 },
    { breakpoint: '1280px', numVisible: 3, numScroll: 1 },
    { breakpoint: '1024px', numVisible: 3, numScroll: 1 },
    { breakpoint: '768px', numVisible: 2, numScroll: 1 }, // phones/tablets → 2 cards
    { breakpoint: '480px', numVisible: 2, numScroll: 1 },
  ];

  @Input() autoplay = false;
  @Input() intervalMs = 3000;

  private timer?: any;
  private touchStartX: number | null = null;

  ngOnInit(): void { if (this.autoplay) this.startAutoplay(); }
  ngOnDestroy(): void { this.stopAutoplay(); }

  next(ev?: MouseEvent) { this.carousel?.navForward(ev ?? this.fakeClick(), this.numScroll); }
  prev(ev?: MouseEvent) { this.carousel?.navBackward(ev ?? this.fakeClick(), this.numScroll); }

  onWheel(e: WheelEvent) {
    // Horizontal preferred; fall back to vertical
    if (Math.abs(e.deltaY) < Math.abs(e.deltaX)) {
      e.deltaX > 0 ? this.carousel?.navForward(e, this.numScroll) : this.carousel?.navBackward(e, this.numScroll);
    } else {
      e.deltaY > 0 ? this.carousel?.navForward(e, this.numScroll) : this.carousel?.navBackward(e, this.numScroll);
    }
    e.preventDefault();
    e.stopPropagation();
  }

  onKeydown(e: KeyboardEvent) {
    if (e.key === 'ArrowRight') { this.carousel?.navForward(this.fakeClick(), this.numScroll); e.preventDefault(); }
    else if (e.key === 'ArrowLeft') { this.carousel?.navBackward(this.fakeClick(), this.numScroll); e.preventDefault(); }
  }

  onTouchStart(e: TouchEvent) { if (e.touches?.length) this.touchStartX = e.touches[0].clientX; }
  onTouchEnd(e: TouchEvent) {
    if (this.touchStartX === null) return;
    const endX = e.changedTouches?.length ? e.changedTouches[0].clientX : this.touchStartX;
    const dx = endX - this.touchStartX;
    const threshold = 24;
    if (dx <= -threshold) this.carousel?.navForward(this.fakeClick(), this.numScroll);
    else if (dx >= threshold) this.carousel?.navBackward(this.fakeClick(), this.numScroll);
    this.touchStartX = null;
  }

  private startAutoplay() { this.stopAutoplay(); this.timer = setInterval(() => this.next(this.fakeClick()), this.intervalMs); }
  private stopAutoplay() { if (this.timer) { clearInterval(this.timer); this.timer = undefined; } }
  private fakeClick() { return new MouseEvent('click', { bubbles: true, cancelable: true }); }
}
