import { Meta, StoryObj } from '@storybook/angular';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, NgControl, ReactiveFormsModule } from '@angular/forms';
import { moduleMetadata } from '@storybook/angular';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { PhotoGalleryComponent } from './photo-gallery.component';

const meta: Meta<PhotoGalleryComponent> = {
  title: 'Components/PhotoGallery',
  component: PhotoGalleryComponent,
  parameters: {
    // More on how to position stories at: https://storybook.js.org/docs/configure/story-layout
    layout: 'fullscreen',
  },
  decorators: [
    moduleMetadata({
      declarations: [DropdownComponent],
      imports: [CommonModule, ReactiveFormsModule, FormsModule],
    }),
  ],
};

export default meta;
type Story = StoryObj<PhotoGalleryComponent>;

let srcs1 = ['assets\\Open Space 1.jpg', 'assets\\Open Space 2.jpg', 'assets\\Open Space 3.jpg', 'assets\\Open Space 4.jpg'];
let srcs2 = ['assets\\Private Room 1.jpg', 'assets\\Private Room 2.jpg', 'assets\\Private Room 3.jpg'];
let srcs3 = ['assets\\Meeting Room 1.jpg', 'assets\\Meeting Room 2.jpg', 'assets\\Meeting Room 3.jpg', 'assets\\Meeting Room 4.jpg'];

let form = new FormControl<Date>(new Date(2025, 5, 15, 12, 0, 0));

export const Workspaces1: Story = {
  args: {
    photoSrcs: srcs1
  }
};
export const Workspaces2: Story = {
  args: {
    photoSrcs: srcs2
  }
};
export const Workspaces3: Story = {
  args: {
    photoSrcs: srcs3
  }
};
