import { Meta, StoryObj } from '@storybook/angular';
import { DropdownComponent } from './dropdown.component';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, NgControl, ReactiveFormsModule } from '@angular/forms';
import { moduleMetadata } from '@storybook/angular';
import { BrowserModule } from '@angular/platform-browser';

const meta: Meta<DropdownComponent<string>> = {
  title: 'Components/Dropdown',
  component: DropdownComponent<string>,
  parameters: {
    // More on how to position stories at: https://storybook.js.org/docs/configure/story-layout
    layout: 'fullscreen',
  },
  decorators: [
    moduleMetadata({
      //declarations: [NgControl],
      imports: [CommonModule, ReactiveFormsModule, FormsModule],
    }),
  ],
};

export default meta;
type Story = StoryObj<DropdownComponent<string>>;

let options = [
 { value: "8:00 AM", text: "8:00 AM" },
 { value: "8:30 AM", text: "8:30 AM" },
 { value: "9:00 AM", text: "9:00 AM" },
 { value: "9:30 AM", text: "9:30 AM" },
 { value: "10:00 AM", text: "10:00 AM" }
];

let form = new FormControl<string>('8:00 AM');

export const Time: Story = {
  args: {
    options: options,
    //value: "8:00 AM",
    //formControl: new FormControl<string>("8:00 AM", {
    //  nonNullable: true
    //})
  },
  render: (args: any) => ({
    template: `<app-dropdown [options]="options" [formControl]="form"></app-dropdown>`,
    props: { ...args, form: form } 
  })
};
