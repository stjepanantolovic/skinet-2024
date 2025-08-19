import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import axios, { type AxiosProgressEvent } from 'axios';

export interface UploadedImage {
  name: string;
  url: string;
  progress: number;
  publicId?: string;
}

@Component({
  selector: 'app-image-uploader',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatProgressBarModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule
  ],
  templateUrl: './image-uploader.component.html',
  styleUrls: ['./image-uploader.component.scss']
})
export class ImageUploaderComponent implements OnInit {
  selectedFiles: File[] = [];
  uploading = false;
  uploadedImages: UploadedImage[] = [];
  displayedColumns: string[] = ['index', 'name', 'preview', 'url', 'progress', 'action'];

  // Dimension UI
  dimensionMode: 'none' | 'preset' | 'custom' = 'preset';
  dimensionPresets = ['490 x 640', '512 x 512', '768 x 768', '1024 x 1024', '1200 x 628', '1920 x 1080'];
  selectedPreset = '490 x 640';
  customDimension = '';
  dimensionError = '';

  // Backend options
  strategy: 'Fit' | 'Pad' = 'Pad';
  padHex = '#FFFFFFFF';
  formatMode: 'Auto' | 'KeepOriginal' | 'ForceJpeg' | 'ForcePng' | 'ForceWebp' = 'Auto';

  ngOnInit(): void {
    this.loadImages();
  }

  onFilesSelected(event: any) {
    this.selectedFiles = Array.from(event.target.files);
  }

  private normalizeDimension(input: string | null | undefined): string | null {
    if (!input) return null;
    const cleaned = input.trim().toLowerCase().replace(/×/g, 'x').replace(/\s+/g, ' ');
    const m = cleaned.match(/^(\d{1,5})\s*x\s*(\d{1,5})$/);
    if (!m) return null;
    const w = +m[1], h = +m[2];
    if (!Number.isFinite(w) || !Number.isFinite(h) || w <= 0 || h <= 0 || w > 10000 || h > 10000) return null;
    return `${w} x ${h}`;
  }

  private currentDimension(): string | null {
    if (this.dimensionMode === 'none') return null;
    const raw = this.dimensionMode === 'preset' ? this.selectedPreset : this.customDimension;
    const norm = this.normalizeDimension(raw);
    if (!norm) {
      this.dimensionError = 'Use format "W x H", e.g. "1024 x 768"';
      return null;
    }
    this.dimensionError = '';
    return norm;
  }

  async uploadFiles() {
    if (this.selectedFiles.length === 0) return;

    const dimension = this.currentDimension();
    if (this.dimensionMode !== 'none' && !dimension) return;

    this.uploading = true;

    for (const file of this.selectedFiles) {
      const formData = new FormData();
      formData.append('file', file);
      if (dimension) formData.append('dimension', dimension);
      formData.append('strategy', this.strategy);
      formData.append('formatMode', this.formatMode);
      if (this.strategy === 'Pad' && this.padHex?.trim()) {
        formData.append('padHex', this.padHex.trim());
      }

      const img: UploadedImage = { name: file.name, url: '', progress: 0 };
      this.uploadedImages.push(img);
      console.log("this.uploadedImages: ", this.uploadedImages);

      try {
        const res = await axios.post('https://localhost:5001/api/images', formData, {
          headers: { 'Content-Type': 'multipart/form-data' },
          onUploadProgress: (pe: AxiosProgressEvent) => {
            const total = pe.total ?? 0;
            const loaded = pe.loaded ?? 0;
            let pct = 0;
            if (total > 0) pct = Math.round((loaded * 100) / total);
            else if (typeof pe.progress === 'number') pct = Math.round(pe.progress * 100);
            img.progress = Math.min(100, Math.max(0, pct));
          }
        });
        img.url = res.data?.url ?? '';
        img.progress = 100;
      } catch (err) {
        console.error('Upload failed', err);
        img.progress = 0;
      }
    }

    this.selectedFiles = [];
    this.uploading = false;
  }

  async loadImages() {
    const res: any = await axios.get('https://localhost:5001/api/images');
    this.uploadedImages = res.data.map((i: any) => ({ ...i, progress: 100 }));
    console.log("this.uploadedImages: ", this.uploadedImages);
  }

  async deleteImage(image: UploadedImage) {
    await axios.delete(`https://localhost:5001/api/images/${encodeURIComponent(image.name)}`);
    this.uploadedImages = this.uploadedImages.filter(i => i.name !== image.name);
    console.log("this.uploadedImages: ", this.uploadedImages);
  }
}
