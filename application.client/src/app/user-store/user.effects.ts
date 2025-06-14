import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { EMPTY } from 'rxjs';
import { map, exhaustMap, catchError } from 'rxjs/operators';
import { ApiService } from '../api.service';
import { getUser, login, updateUser } from './user.actions';

@Injectable()
export class UserEffects
{
  private actions$ = inject(Actions);
  private api = inject(ApiService);

  getUser$ = createEffect(() =>
  {
    return this.actions$.pipe(
      ofType(getUser),
      exhaustMap(() => this.api.getUser()
        .pipe(
          map(response => updateUser({ dto: response }))
        ))
    );
  });

  login$ = createEffect(() =>
  {
    return this.actions$.pipe(
      ofType(login),
      exhaustMap((action) => this.api.login(action.dto)
        .pipe(
          map(_ => getUser())
        ))
    );
  });
}
