import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NavigationComponent } from './navigation/navigation.component';
import { BookingErrorComponent } from './booking-error/booking-error.component';
import { BookingFormComponent } from './booking-form/booking-form.component';
import { BookingSuccessComponent } from './booking-success/booking-success.component';
import { CoworkingDetailsComponent } from './coworking-details/coworking-details.component';
import { DatePickerComponent } from './date-picker/date-picker.component';
import { EditBookingFormComponent } from './edit-booking-form/edit-booking-form.component';
import { MyBookingComponent } from './my-booking/my-booking.component';
import { PhotoGalleryComponent } from './photo-gallery/photo-gallery.component';
import { DeleteBookingComponent } from './delete-booking/delete-booking.component';
import { DropdownComponent } from './dropdown/dropdown.component';
import { ModalComponent } from './modal/modal.component';

@NgModule({
  declarations: [
    AppComponent,
    BookingErrorComponent,
    BookingFormComponent,
    BookingSuccessComponent,
    CoworkingDetailsComponent,
    DatePickerComponent,
    DeleteBookingComponent,
    EditBookingFormComponent,
    MyBookingComponent,
    NavigationComponent,
    PhotoGalleryComponent,
    DropdownComponent,
    ModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
