import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
import { NgxGalleryModule } from 'ngx-gallery';

import { AlertifyService } from './#services/alertify.service';
import { AuthGuard } from './#guards/auth.guard';
import { AuthService } from './#services/auth.service';
import { ErrorInterceptorProvider } from './#interceptors/error.interceptor';
import { UserService } from './#services/user.service';

import { AppComponent } from './app.component';
import { ROUTES } from './app.routes';
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from './header/header.component';
import { ListsComponent } from './components/lists/lists.component';
import { MessagesComponent } from './components/messages/messages.component';
import { RegisterComponent } from './components/home/register/register.component';
import { MemberListComponent } from './components/members/member-list/member-list.component';
import { MemberCardComponent } from './components/members/member-card/member-card.component';
import { JwtModule } from '@auth0/angular-jwt';
import { MemberDetailComponent } from './components/members/member-detail/member-detail.component';
import { MemberDetailResolver } from './#resolvers/member-detail.resolver';
import { MemberListResolver } from './#resolvers/member-list.resolver';
import { MemberEditComponent } from './components/members/member-edit/member-edit.component';
import { MemberEditResolver } from './#resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './#guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './components/members/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeAgoPipe } from 'time-ago-pipe';
import { environment } from 'src/environments/environment';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
    RegisterComponent,
    ListsComponent,
    MessagesComponent,
    MemberListComponent,
    MemberCardComponent,
    MemberDetailComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    TimeAgoPipe
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationModule.forRoot(),
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    ButtonsModule.forRoot(),
    NgxGalleryModule,
    FileUploadModule,
    TabsModule.forRoot(),
    RouterModule.forRoot(ROUTES),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: [environment.base_url],
        blacklistedRoutes: [`${environment.base_url}/auth`]
      }
    })
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AuthGuard,
    PreventUnsavedChanges,
    AlertifyService,
    UserService,
    MemberDetailResolver,
    MemberListResolver,
    MemberEditResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
