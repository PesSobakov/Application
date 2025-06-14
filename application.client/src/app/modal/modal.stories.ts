import {  type Meta, type StoryObj } from '@storybook/angular';
import { ModalComponent } from './modal.component';

const meta: Meta<ModalComponent> = {
  title: 'Components/Modal',
  component: ModalComponent,
  parameters: {
    // More on how to position stories at: https://storybook.js.org/docs/configure/story-layout
    layout: 'fullscreen',
  },
};

export default meta;
type Story = StoryObj<ModalComponent>;

export const Shown: Story = {
  args: {
    opened: true
  },
  render: (args: any) => ({
    template: `<app-modal [opened]="${args.opened}">Content</app-modal>`
  })
};
