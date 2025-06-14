import { environment } from './../../environments/environment';

import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { filter, map, shareReplay } from 'rxjs/operators';
import { ActivatedRoute, NavigationEnd, NavigationStart, Router } from '@angular/router';
import { ApiService } from '../api.service';
import { LoginDto } from '../../DTOs/LoginDto';
import { Store } from '@ngrx/store';
import { StringDto } from '../../DTOs/StringDto';
import { getUser, login } from '../user-store/user.actions';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css',
  standalone: false
})
export class NavigationComponent
{
  user$: Observable<StringDto | undefined>;
  constructor(
    private store: Store<{ user: StringDto | undefined }>,
    private router: Router
  )
  {
    this.user$ = store.select('user');
    router.events.pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe({
        next: (e: NavigationEnd) =>
        {
          this.url = e.url;
          this.updateUser();
        }
      });
    router.events.pipe(filter((e): e is NavigationStart => e instanceof NavigationStart))
      .subscribe({
        next: (e: NavigationStart) =>
        {
          this.url = e.url;
          this.updateUser();
        }
      })

  }
  /*
  private breakpointObserver = inject(BreakpointObserver);

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  accountInfo?: AccountInfo;*/

  url: string = "";
  checkNav(nav: string)
  {
    return this.url.includes(nav);
  }
  setSelectedNav(nav: string)
  {
    return this.url.includes(nav) ? "border-b-[1.5px] border-[#5C5AF3]" : "";
  }
  isSelectedNav(nav: string)
  {
    return this.url.includes(nav);
  }


  username = "";
  login()
  {
    this.store.dispatch(login({ dto: <LoginDto>{ email: this.username } }));
  }

  updateUser()
  {
    this.store.dispatch(getUser());
  }

  ngOnInit()
  {
    this.user$.subscribe((user) =>
    {
      if (user == undefined) {
        this.updateUser();
      }
    });
  }
}
