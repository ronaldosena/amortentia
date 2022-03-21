import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { AuthGuard } from './#guards/auth.guard';
import { MessagesComponent } from './components/messages/messages.component';
import { ListsComponent } from './components/lists/lists.component';
import { MemberListComponent } from './components/members/member-list/member-list.component';
import { MemberDetailComponent } from './components/members/member-detail/member-detail.component';
import { MemberDetailResolver } from './#resolvers/member-detail.resolver';
import { MemberListResolver } from './#resolvers/member-list.resolver';
import { MemberEditComponent } from './components/members/member-edit/member-edit.component';
import { MemberEditResolver } from './#resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './#guards/prevent-unsaved-changes.guard';

export const ROUTES: Routes = [
  { path: '', component: HomeComponent },
  // { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
  // Or, for multiple guards at once
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'members',
        component: MemberListComponent,
        resolve: { users: MemberListResolver }
      },
      {
        path: 'members/:username',
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver }
      },
      {
        path: 'member/edit',
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnsavedChanges]
      },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent }
    ]
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
