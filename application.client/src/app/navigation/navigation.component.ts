import { environment } from './../../environments/environment';

import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { filter, map, shareReplay } from 'rxjs/operators';
import { ActivatedRoute, NavigationEnd, NavigationStart, Router } from '@angular/router';
import { ApiService } from '../api.service';
import { LoginDto } from '../../DTOs/LoginDto';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent
{
  constructor(
    private api: ApiService,
    private route: ActivatedRoute,
    private router: Router
  )
  {
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

  user$ = this.api.getUser();

  username = "";
  login()
  {
    this.api.login(<LoginDto>{ email: this.username }).subscribe(() =>
    {
      this.updateUser();
      const currentUrl = this.router.url;
      this.router.navigateByUrl('/dummy', { skipLocationChange: true }).then(() =>
      {
        this.router.navigate([currentUrl]);
      });
    });

  }

  updateUser()
  {
    this.user$ = this.api.getUser();
  }

  ngOnInit()
  {
    this.updateUser();
  }

}
