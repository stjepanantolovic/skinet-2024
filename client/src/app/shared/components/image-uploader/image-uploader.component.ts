import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import axios from 'axios';

export interface UploadedImage {
  name: string;
  url: string;
  progress: number;  // 0–100 for upload progress
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
    MatTableModule
  ],
  templateUrl: './image-uploader.component.html',
  styleUrls: ['./image-uploader.component.scss']
})
export class ImageUploaderComponent implements OnInit {

  selectedFiles: File[] = [];
  uploading = false;
  uploadedImages: UploadedImage[] = [];
  displayedColumns: string[] = ['index', 'name', 'preview', 'url', 'progress', 'action'];

  ngOnInit(): void {
    this.loadImages();
  }

  onFilesSelected(event: any) {
    this.selectedFiles = Array.from(event.target.files);
  }

  async uploadFiles() {
    if (this.selectedFiles.length === 0) return;
    this.uploading = true;

    for (let file of this.selectedFiles) {
      const formData = new FormData();
      formData.append('file', file);

      const img: UploadedImage = { name: file.name, url: '', progress: 0 };
      this.uploadedImages.push(img);

      await axios.post('https://localhost:5001/api/images', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
        onUploadProgress: progressEvent => {
          if (progressEvent && progressEvent.total) {
            img.progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          }
        }
      }).then(res => {
        img.url = res.data.url;
        img.progress = 100;
      });
    }

    this.selectedFiles = [];
    this.uploading = false;
  }

  async loadImages() {
    const res: any = await axios.get('https://localhost:5001/api/images');
    this.uploadedImages = res.data.map((i: any) => ({ ...i, progress: 100 }));
  }

  async deleteImage(image: UploadedImage) {
    await axios.delete(`https://localhost:5001/api/images/${image.name}`);
    this.uploadedImages = this.uploadedImages.filter(i => i.name !== image.name);
  }
}
