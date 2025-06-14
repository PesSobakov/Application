import { Component, Input } from '@angular/core';
import { WorkspaceType } from '../../models/WorkspaceType';
import { Option } from '../../models/Option';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { CreateBookingDto } from '../../DTOs/CreateBookingDto';
import { ApiService } from '../api.service';
import { HttpErrorResponse } from '@angular/common/http';
import { BookingDto } from '../../DTOs/GetBooking/BookingDto';
import { EditBookingDto } from '../../DTOs/EditBookingDto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-booking-form',
  templateUrl: './edit-booking-form.component.html',
  styleUrl: './edit-booking-form.component.css',
  standalone: false
})
export class EditBookingFormComponent
{
  constructor(private fb: FormBuilder, private api: ApiService, private router: Router) { }
  workspaceTypeOptions = [
    <Option<WorkspaceType>>{ value: WorkspaceType.OpenSpace, text: "Open Space" },
    <Option<WorkspaceType>>{ value: WorkspaceType.PrivateRoom, text: "Private room" },
    <Option<WorkspaceType>>{ value: WorkspaceType.MeetingRoom, text: "Meeting room" }
  ];
  //new Date(0,0,0,9,0,0).toTimeString()
  timeSlotBoundaries = [
    <Option<Date>>{ value: new Date(0, 0, 0, 8, 0, 0), text: "8:00 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 8, 30, 0), text: "8:30 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 9, 0, 0), text: "9:00 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 9, 30, 0), text: "9:30 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 10, 0, 0), text: "10:00 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 10, 30, 0), text: "10:30 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 11, 0, 0), text: "11:00 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 11, 30, 0), text: "11:30 AM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 12, 0, 0), text: "12:00 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 12, 30, 0), text: "12:30 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 13, 0, 0), text: "1:00 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 13, 30, 0), text: "1:30 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 14, 0, 0), text: "2:00 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 14, 30, 0), text: "2:30 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 15, 0, 0), text: "3:00 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 15, 30, 0), text: "3:30 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 16, 0, 0), text: "4:00 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 16, 30, 0), text: "4:30 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 17, 0, 0), text: "5:00 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 17, 30, 0), text: "5:30 PM" },
    <Option<Date>>{ value: new Date(0, 0, 0, 18, 0, 0), text: "6:00 PM" },
  ];
  startTimeOptions = this.timeSlotBoundaries.slice(0, -1);
  endTimeOptions = this.timeSlotBoundaries.slice(1);

  @Input() id: string | undefined

  onSelectedWorkspaceTypeChange()
  {
    this.createBookingForm.patchValue({
      seats: null
    });

  }
  onSelectedStartTimeChange()
  {
    if (this.createBookingForm.value.startTime != undefined) {
      let option = this.timeSlotBoundaries.find((x) => x.value.getTime() == this.createBookingForm.value.startTime?.getTime());
      if (option != undefined) {
        this.endTimeOptions = this.timeSlotBoundaries.slice(this.timeSlotBoundaries.indexOf(option) + 1);
      }
      if (this.createBookingForm.value.endTime != undefined) {
        if (this.createBookingForm.value.startTime >= this.createBookingForm.value.endTime) {
          this.createBookingForm.patchValue({
            endTime: this.endTimeOptions[0].value
          });
        }
      }
    }
  }

  minStartDate: Date = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0);
  maxStartDate: Date = new Date(new Date().getFullYear() + 5, new Date().getMonth(), new Date().getDate(), 12, 0, 0);
  minEndDate: Date = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0);
  maxEndDate: Date = new Date(new Date().getFullYear() + 5, new Date().getMonth(), new Date().getDate(), 12, 0, 0);


  onSelectedStartDateChange()
  {
    if (this.createBookingForm.value.startDate != undefined) {
      if (this.createBookingForm.value.workspaceType != undefined) {
        if (this.createBookingForm.value.workspaceType == WorkspaceType.MeetingRoom) {
          this.minEndDate = this.createBookingForm.value.startDate;
          this.maxEndDate = this.createBookingForm.value.startDate;
        }
        else {
          this.minEndDate = this.createBookingForm.value.startDate;
          this.maxEndDate = new Date(this.createBookingForm.value.startDate.getFullYear(), this.createBookingForm.value.startDate.getMonth(), this.createBookingForm.value.startDate.getDate() + 29, 12, 0, 0);
        }
      }
      else {
        this.minEndDate = this.createBookingForm.value.startDate;
        this.maxEndDate = new Date(this.createBookingForm.value.startDate.getFullYear(), this.createBookingForm.value.startDate.getMonth(), this.createBookingForm.value.startDate.getDate() + 29, 12, 0, 0);
      }
    }
  }


  createBookingForm = this.fb.group(
    {
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      workspaceType: new FormControl<WorkspaceType | null>(null, {
        validators: [Validators.required]
      }),
      seats: new FormControl<number | null>(null, {
        validators: [Validators.required, Validators.min(1)]
      }),
      startDate: new FormControl<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0), {
        validators: [Validators.required]
      }),
      endDate: new FormControl<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0), {
        validators: [Validators.required]
      }),
      startTime: new FormControl<Date | null>(null, {
        validators: [Validators.required]
      }),
      endTime: new FormControl<Date | null>(null, {
        validators: [Validators.required]
      })
    }
  );

  get name()
  {
    return this.createBookingForm.get('name')!;
  }
  get email()
  {
    return this.createBookingForm.get('email')!;
  }
  get workspaceType()
  {
    return this.createBookingForm.get('workspaceType')!;
  }
  get seats()
  {
    return this.createBookingForm.get('seats')!;
  }
  get startDate()
  {
    return this.createBookingForm.get('startDate')!;
  }
  get endDate()
  {
    return this.createBookingForm.get('endDate')!;
  }
  get startTime()
  {
    return this.createBookingForm.get('startTime')!;
  }
  get endTime()
  {
    return this.createBookingForm.get('endTime')!;
  }

  workspaceTypes = Object.values(WorkspaceType);





  error: string | undefined;
  errors: string[] = [];
  sentDto: EditBookingDto | undefined;
  successOpened = false;
  errorOpened = false;

  onSubmit()
  {
    if (this.id != undefined) {
      this.errors = [];
      if (this.createBookingForm.valid) {
        let dto = <EditBookingDto>{
          name: this.createBookingForm.value.name!,
          email: this.createBookingForm.value.email,
          workspaceType: this.createBookingForm.value.workspaceType,
          seats: this.createBookingForm.value.seats,
          startDate: this.createBookingForm.value.startDate?.toISOString().slice(0, 10),
          endDate: this.createBookingForm.value.endDate?.toISOString().slice(0, 10),
          startTime: this.createBookingForm.value.startTime?.toTimeString().slice(0, 8),
          endTime: this.createBookingForm.value.endTime?.toTimeString().slice(0, 8),
        }
        this.sentDto = dto;
        this.api.EditBooking(+(this.id), dto).subscribe({
          complete: () => { this.router.navigate(["/my-booking"]) },
          error: (error: HttpErrorResponse) =>
          {
            if (error?.error?.errors != undefined) {
              let errorNames = Object.getOwnPropertyNames(error.error.errors)
              if (errorNames.length == 1) {
                this.errors = [error?.error?.errors[errorNames[0]]];
              }
              else {
                for (var i = 0; i < errorNames.length; i++) {
                  this.errors.push(`${errorNames[i]}: ${error?.error?.errors[errorNames[i]]}`)
                }
              }
            }
            this.errorOpened = true
          },
        });
      }
    }
    else {
      this.error = "Id is required";
    }
  }

  ngOnInit()
  {
    if (this.id != undefined) {
      let id = +this.id;
      this.api.GetBooking(id).subscribe({
        next: (x) =>
        {
          this.createBookingForm.setValue({
            name: x.name,
            email: x.email,
            workspaceType: x.workspace.workspaceType,
            seats: x.seats,
            startDate: new Date(new Date(x.startDate).getFullYear(), new Date(x.startDate).getMonth(), new Date(x.startDate).getDate(), 12, 0, 0),
            endDate: new Date(new Date(x.endDate).getFullYear(), new Date(x.endDate).getMonth(), new Date(x.endDate).getDate(), 12, 0, 0),
            startTime: new Date(0, 0, 0, +(x.startTime.slice(0, 2)), +(x.startTime.slice(3, 5)), 0),
            endTime: new Date(0, 0, 0, +(x.endTime.slice(0, 2)), +(x.endTime.slice(3, 5)), 0),
          });

          if (this.createBookingForm.value.startDate != undefined) {
            if (this.createBookingForm.value.workspaceType != undefined) {
              if (this.createBookingForm.value.workspaceType == WorkspaceType.MeetingRoom) {
                this.minEndDate = this.createBookingForm.value.startDate;
                this.maxEndDate = this.createBookingForm.value.startDate;
              }
              else {
                this.minEndDate = this.createBookingForm.value.startDate;
                this.maxEndDate = new Date(this.createBookingForm.value.startDate.getFullYear(), this.createBookingForm.value.startDate.getMonth(), this.createBookingForm.value.startDate.getDate() + 29, 12, 0, 0);
              }
            }
            else {
              this.minEndDate = this.createBookingForm.value.startDate;
              this.maxEndDate = new Date(this.createBookingForm.value.startDate.getFullYear(), this.createBookingForm.value.startDate.getMonth(), this.createBookingForm.value.startDate.getDate() + 29, 12, 0, 0);
            }
          }
        },
        error: () =>
        {
          this.error = "Error loading booking";
        },
      });
    }
    else {
      this.error = "Id is required";
    }
  }
}
