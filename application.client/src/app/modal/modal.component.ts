import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.css',
  standalone:false
})
export class ModalComponent
{
  @Input() opened: boolean = false;
  @Output() openedChange = new EventEmitter<boolean>();

  clickOutside(event: MouseEvent)
  {
    if (event.target === event.currentTarget) {
      this.opened = false;
    }
  }

  close()
  {
    this.opened = false;
  }


}
