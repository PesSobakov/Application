import {  NgModule } from '@angular/core';
import { ExtraOptions, RouterModule, Routes } from '@angular/router';
import { CoworkingDetailsComponent } from './coworking-details/coworking-details.component';
import { BookingFormComponent } from './booking-form/booking-form.component';
import { MyBookingComponent } from './my-booking/my-booking.component';
import { EditBookingFormComponent } from './edit-booking-form/edit-booking-form.component';

const routes: Routes = [
  {
    path: 'coworking-details',
    component: CoworkingDetailsComponent,
  },
  {
    path: 'booking-form/:type',
    component: BookingFormComponent
  },
  {
    path: 'booking-form',
    component: BookingFormComponent
  },
  {
    path: 'my-booking',
    component: MyBookingComponent
  },
  {
    path: 'edit-booking-form/:id',
    component: EditBookingFormComponent
  },

  { path: '', redirectTo: '/coworking-details', pathMatch: 'full' },
  { path: "**", component: CoworkingDetailsComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, <ExtraOptions>{ bindToComponentInputs: true, onSameUrlNavigation: 'reload',scrollPositionRestoration:'top' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
