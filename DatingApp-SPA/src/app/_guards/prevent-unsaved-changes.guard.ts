import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

/**
 * ANGULAR: NAVIGATION [CanDeactivate]
 *
 * NOTE: This guard is attached to only MemberEditComponent
 *
 * Interface that a class can implement to be a guard deciding if a route can be deactivated.
 * If all guards return true, navigation will continue. If any guard returns false,
 * navigation will be cancelled. If any guard returns a UrlTree, current navigation will be
 * cancelled and a new navigation will be kicked off to the UrlTree returned from the guard.
 */
@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {
  canDeactivate(component: MemberEditComponent) {
    if (component.editForm.dirty) {
        return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }
}
