import { createReducer, on } from '@ngrx/store';
import { updateUser } from './user.actions';
import { StringDto } from '../../DTOs/StringDto';

export const initialState = undefined;

export const userReducer = createReducer<StringDto | undefined>(
  initialState,
  on(updateUser, (_,action) => action.dto )
);
