import { Meta, StoryObj } from '@storybook/angular';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, NgControl, ReactiveFormsModule } from '@angular/forms';
import { moduleMetadata } from '@storybook/angular';
import { DatePickerComponent } from './date-picker.component';
import { DropdownComponent } from '../dropdown/dropdown.component';

const meta: Meta<DatePickerComponent> = {
  title: 'Components/DatePicker',
  component: DatePickerComponent,
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
type Story = StoryObj<DatePickerComponent>;

let options = [
 { value: "8:00 AM", text: "8:00 AM" },
 { value: "8:30 AM", text: "8:30 AM" },
 { value: "9:00 AM", text: "9:00 AM" },
 { value: "9:30 AM", text: "9:30 AM" },
 { value: "10:00 AM", text: "10:00 AM" }
];

let form = new FormControl<Date>(new Date(2025, 5, 15, 12, 0, 0));

export const DatePick: Story = {
  args: {
    minDate: new Date(2025, 5, 14, 12, 0, 0),
    maxDate: new Date(2025, 6, 14, 12, 0, 0),
    //value: "8:00 AM",
    //formControl: new FormControl<string>("8:00 AM", {
    //  nonNullable: true
    //})
  },
  render: (args: any) => ({
    template: `<app-date-picker [minDate]="minDate" [maxDate]="maxDate" [formControl]="form"></app-date-picker>`,
    props: { ...args, form: form } 
  })
};
