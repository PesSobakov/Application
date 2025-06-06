import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-photo-gallery',
  templateUrl: './photo-gallery.component.html',
  styleUrl: './photo-gallery.component.css'
})
export class PhotoGalleryComponent {
  @Input() photoSrcs: string[] = [];
  selected = 0;

}
