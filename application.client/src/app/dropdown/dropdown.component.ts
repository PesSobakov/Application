import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Optional, Output, Self } from '@angular/core';
import { Option } from '../../models/Option';
import { ControlValueAccessor, NgControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-dropdown',
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.css',
  standalone: false
})
export class DropdownComponent<T> implements OnInit, ControlValueAccessor
{
  constructor(private elementRef: ElementRef, @Self() public controlDir: NgControl)
  {
    controlDir.valueAccessor = this;
  }

  @Input() options: Option<T>[] = [];
  opened: boolean = false;
  @Input() value: T | undefined;
  @Output() valueChange = new EventEmitter<T>();
  selected: Option<T> | undefined;

  onSelectedChanged(option: Option<T>)
  {
    this.value = option.value;
    this.onChange(option.value);
    this.onTouched();
    this.opened = false;
    this.valueChange.emit(option.value);
    if (this.value instanceof Date) {
      if (this.value != undefined) {
        this.selected = this.options.find((x) => (x.value as Date).getTime() == (this.value as Date).getTime());
      }
    }
    else {
      if (this.value != undefined) {
        this.selected = this.options.find((x) => x.value == this.value);
      }
    }

  }

  toggleOpened()
  {
    this.opened = !this.opened;
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event)
  {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.opened = false;
    }
  }

  @Output() blur: EventEmitter<void> = new EventEmitter<void>();
  onChange: (value: any) => void = () => { };
  onTouched: () => void = () => { };

  ngOnInit(): void
  {
    const control = this.controlDir.control;
    const validators = control?.validator
      ? [control.validator, Validators.required]
      : Validators.required;
    control?.setValidators(validators);
    if (this.value instanceof Date) {
      if (this.value != undefined) {
        this.selected = this.options.find((x) => (x.value as Date).getTime() == (this.value as Date).getTime());
      }
    }
    else {
      if (this.value != undefined) {
        this.selected = this.options.find((x) => x.value == this.value);
      }
    }
  }

  ngOnChanges()
  {
    if (this.value instanceof Date) {
      if (this.value != undefined) {
        this.selected = this.options.find((x) => (x.value as Date).getTime() == (this.value as Date).getTime());
      }
    }
    else {
      if (this.value != undefined) {
        this.selected = this.options.find((x) => x.value == this.value);
      }
    }
  }

  writeValue(value: any): void
  {
    if (value != undefined) {
      //this.controlDir.control?.setValue(value, { emitEvent: false });
      this.value = value;
      if (this.value instanceof Date) {
          this.selected = this.options.find((x) => (x.value as Date).getTime() == (this.value as Date).getTime());
      }
      else {
          this.selected = this.options.find((x) => x.value == this.value);
      }
    }

  }

  registerOnChange(onChange: (value: any) => void): void
  {
    this.onChange = onChange;
  }

  registerOnTouched(onTouched: () => void): void
  {
    this.onTouched = onTouched;
  }
}
