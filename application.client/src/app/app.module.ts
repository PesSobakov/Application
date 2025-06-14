import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NavigationComponent } from './navigation/navigation.component';
import { BookingFormComponent } from './booking-form/booking-form.component';
import { CoworkingDetailsComponent } from './coworking-details/coworking-details.component';
import { DatePickerComponent } from './date-picker/date-picker.component';
import { EditBookingFormComponent } from './edit-booking-form/edit-booking-form.component';
import { MyBookingComponent } from './my-booking/my-booking.component';
import { PhotoGalleryComponent } from './photo-gallery/photo-gallery.component';
import { DropdownComponent } from './dropdown/dropdown.component';
import { ModalComponent } from './modal/modal.component';
import { CoworkingListComponent } from './coworking-list/coworking-list.component';
import { AiAssistantComponent } from './ai-assistant/ai-assistant.component';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { userReducer } from './user-store/user.reducer';
import { UserEffects } from './user-store/user.effects';

@NgModule({
  declarations: [
    AppComponent,
    BookingFormComponent,
    CoworkingDetailsComponent,
    DatePickerComponent,
    EditBookingFormComponent,
    MyBookingComponent,
    NavigationComponent,
    PhotoGalleryComponent,
    DropdownComponent,
    ModalComponent,
    CoworkingListComponent,
    AiAssistantComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    StoreModule.forRoot({ user: userReducer }),
    EffectsModule.forRoot([UserEffects])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
