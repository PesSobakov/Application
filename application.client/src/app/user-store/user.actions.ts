import { createAction, props } from '@ngrx/store';
import { LoginDto } from '../../DTOs/LoginDto';
import { StringDto } from '../../DTOs/StringDto';

export const getUser = createAction('Get User');
export const login = createAction('Login User', props<{ dto: LoginDto }>());

export const updateUser = createAction('Update User', props<{ dto: StringDto }>());
