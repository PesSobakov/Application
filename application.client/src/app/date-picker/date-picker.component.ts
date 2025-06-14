import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Optional, Output, Self, SimpleChanges } from '@angular/core';
import { Option } from '../../models/Option';
import { ControlValueAccessor, FormControl, NgControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrl: './date-picker.component.css',
  standalone: false
})
export class DatePickerComponent implements OnInit, ControlValueAccessor
{
  constructor(private elementRef: ElementRef, @Self() public controlDir: NgControl)
  {
    controlDir.valueAccessor = this;
  }

  monthBaseOptions: Option<number>[] = [
    { value: 0, text: "January" },
    { value: 1, text: "February" },
    { value: 2, text: "March" },
    { value: 3, text: "April" },
    { value: 4, text: "May" },
    { value: 5, text: "June" },
    { value: 6, text: "July" },
    { value: 7, text: "August" },
    { value: 8, text: "September" },
    { value: 9, text: "October" },
    { value: 10, text: "November" },
    { value: 11, text: "December" },
  ]

  @Input() minDate: Date = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0);
  @Input() maxDate: Date = new Date(new Date().getFullYear() + 1, new Date().getMonth(), new Date().getDate(), 12, 0, 0);
  @Input() value: Date = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0)
  @Output() valueChange = new EventEmitter<Date>();

  //year!: number
  //month!: number
  //day!: number

  yearControl = new FormControl<number>(0, {
    validators: [Validators.required],
    nonNullable: true
  });
  monthControl = new FormControl<number>(0, {
    validators: [Validators.required],
    nonNullable: true
  });
  dayControl = new FormControl<number>(0, {
    validators: [Validators.required],
    nonNullable: true
  });

  yearOptions: Option<number>[] = [];
  monthOptions: Option<number>[] = [];
  dayOptions: Option<number>[] = [];



  @Output() blur: EventEmitter<void> = new EventEmitter<void>();
  onChange: (value: any) => void = () => { };
  onTouched: () => void = () => { };


  fixWrongDate()
  {
    let daysInMonth = new Date(this.yearControl.value, this.monthControl.value + 1, 0, 12, 0, 0).getDate();
    if (this.dayControl.value > daysInMonth) {
      this.dayControl.setValue(daysInMonth);
    }

    let minMonth;
    let maxMonth;
    if (this.minDate.getFullYear() == this.maxDate.getFullYear()) {
      minMonth = this.minDate.getMonth();
      maxMonth = this.maxDate.getMonth();
    }
    else if (this.value.getFullYear() == this.minDate.getFullYear()) {
      minMonth = this.minDate.getMonth();
      maxMonth = 11;
    }
    else if (this.value.getFullYear() == this.maxDate.getFullYear()) {
      minMonth = 0;
      maxMonth = this.maxDate.getMonth();
    }
    else {
      minMonth = 0;
      maxMonth = 11;
    }
    if (this.monthControl.value < minMonth) {
      this.monthControl.setValue(minMonth);
    }
    else if (this.monthControl.value > maxMonth) {
      this.monthControl.setValue(maxMonth);
    }

    let minDay;
    let maxDay
    if (this.minDate.getFullYear() == this.maxDate.getFullYear() && this.minDate.getMonth() == this.maxDate.getMonth()) {
      minDay = this.minDate.getDate();
      maxDay = this.maxDate.getDate();
    }
    else if (this.value.getFullYear() == this.minDate.getFullYear() && this.value.getMonth() == this.minDate.getMonth()) {
      minDay = this.minDate.getDate();
      maxDay = new Date(this.value.getFullYear(), this.value.getMonth() + 1, 0, 12, 0, 0).getDate();
    }
    else if (this.value.getFullYear() == this.maxDate.getFullYear() && this.value.getMonth() == this.maxDate.getMonth()) {
      minDay = 1;
      maxDay = this.maxDate.getDate();
    }
    else {
      minDay = 1;
      maxDay = new Date(this.value.getFullYear(), this.value.getMonth() + 1, 0, 12, 0, 0).getDate();
    }
    if (this.dayControl.value < minDay) {
      this.dayControl.setValue(minDay);
    }
    else if (this.dayControl.value > maxDay) {
      this.dayControl.setValue(maxDay);
    }

    let newDate = new Date(this.yearControl.value, this.monthControl.value, this.dayControl.value, 12, 0, 0);
    if (newDate < this.minDate) {
      this.yearControl.setValue(this.minDate.getFullYear());
      this.monthControl.setValue(this.minDate.getMonth());
      this.dayControl.setValue(this.minDate.getDate());
      this.value = this.minDate;
    }
    else if (newDate > this.maxDate) {
      this.yearControl.setValue(this.maxDate.getFullYear());
      this.monthControl.setValue(this.maxDate.getMonth());
      this.dayControl.setValue(this.maxDate.getDate());
      this.value = this.maxDate;
    }
    else {
      this.value = newDate;
    }
  }

  onYearChanged()
  {
    this.fixWrongDate();
 this.updateMonthDropdown();
    this.updateDayDropdown();
    
    this.onChange(this.value);
    this.onTouched();
    this.valueChange.emit(this.value);
  }
  onMonthChanged()
  {
    this.fixWrongDate();
    this.updateDayDropdown();

    this.onChange(this.value);
    this.onTouched();
    this.valueChange.emit(this.value);
  }
  onDayChanged()
  {
    this.fixWrongDate();

    this.onChange(this.value);
    this.onTouched();
    this.valueChange.emit(this.value);
  }

  updateYearDropdown()
  {
    this.yearOptions = [];
    for (var i = this.minDate.getFullYear(); i <= this.maxDate.getFullYear(); i++) {
      this.yearOptions.push(<Option<number>>{ value: i, text: i.toString() });
    }
  }
  updateMonthDropdown()
  {
    this.monthOptions = [];
    if (this.minDate.getFullYear() == this.maxDate.getFullYear()) {
      this.monthOptions = this.monthBaseOptions.slice(this.minDate.getMonth(), this.maxDate.getMonth() + 1);
    }
    else if (this.value.getFullYear() == this.minDate.getFullYear()) {
      this.monthOptions = this.monthBaseOptions.slice(this.minDate.getMonth());
    }
    else if (this.value.getFullYear() == this.maxDate.getFullYear()) {
      this.monthOptions = this.monthBaseOptions.slice(0, this.maxDate.getMonth() + 1);
    }
    else {
      this.monthOptions = this.monthBaseOptions.slice(0);
    }
  }
  updateDayDropdown()
  {
    this.dayOptions = [];
    if (this.minDate.getFullYear() == this.maxDate.getFullYear() && this.minDate.getMonth() == this.maxDate.getMonth()) {
      for (var i = this.minDate.getDate(); i <= this.maxDate.getDate(); i++) {
        this.dayOptions.push(<Option<number>>{ value: i, text: i.toString() });
      }
    }
    else if (this.value.getFullYear() == this.minDate.getFullYear() && this.value.getMonth() == this.minDate.getMonth()) {
      for (var i = this.minDate.getDate(); i <= new Date(this.value.getFullYear(), this.value.getMonth() + 1, 0, 12, 0, 0).getDate(); i++) {
        this.dayOptions.push(<Option<number>>{ value: i, text: i.toString() });
      }
    }
    else if (this.value.getFullYear() == this.maxDate.getFullYear() && this.value.getMonth() == this.maxDate.getMonth()) {
      for (var i = 1; i <= this.maxDate.getDate(); i++) {
        this.dayOptions.push(<Option<number>>{ value: i, text: i.toString() });
      }
    }
    else {
      for (var i = 1; i <= new Date(this.value.getFullYear(), this.value.getMonth() + 1, 0, 12, 0, 0).getDate(); i++) {
        this.dayOptions.push(<Option<number>>{ value: i, text: i.toString() });
      }
    }
  }

  ngOnChanges(changes: SimpleChanges)
  {

    if (this.value < this.minDate) {
      this.value = this.minDate
      this.onChange(this.value);
      this.onTouched();
      this.valueChange.emit(this.value);
    }
    if (this.value > this.maxDate) {
      this.value = this.maxDate
      this.onChange(this.value);
      this.onTouched();
      this.valueChange.emit(this.value);

    }

    this.updateYearDropdown();
    this.updateMonthDropdown();
    this.updateDayDropdown();

    this.yearControl.setValue(this.value.getFullYear());
    this.monthControl.setValue(this.value.getMonth());
    this.dayControl.setValue(this.value.getDate());
  }


  ngOnInit(): void
  {
    this.yearControl.setValue(this.value.getFullYear());
    this.monthControl.setValue(this.value.getMonth());
    this.dayControl.setValue(this.value.getDate());

    if (this.value < this.minDate) {
      this.value = this.minDate
      this.onChange(this.value);
      this.onTouched();
      this.valueChange.emit(this.value);
    }
    if (this.value > this.maxDate) {
      this.value = this.maxDate
      this.onChange(this.value);
      this.onTouched();
      this.valueChange.emit(this.value);
    }

    this.updateYearDropdown();
    this.updateMonthDropdown();
    this.updateDayDropdown();
  }

  writeValue(value: any): void
  {
    if (value != undefined) {
      this.value = value;

      this.updateYearDropdown();
      this.updateMonthDropdown();
      this.updateDayDropdown();

      this.yearControl.setValue(this.value.getFullYear());
      this.monthControl.setValue(this.value.getMonth());
      this.dayControl.setValue(this.value.getDate());
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
