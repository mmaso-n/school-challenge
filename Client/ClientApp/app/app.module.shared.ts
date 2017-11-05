import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { TeachersComponent } from './components/teachers/teachers.component';
import { UploadComponent } from './components/upload/upload.component';
import { InsertStudentComponent } from './components/insertstudent/insertstudent.component';
import { InsertTeacherComponent } from './components/insertteacher/insertteacher.component';

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        TeachersComponent,
        FetchDataComponent,
        HomeComponent,
        UploadComponent,
        InsertStudentComponent,
        InsertTeacherComponent
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'teachers', component: TeachersComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'upload', component: UploadComponent },
            { path: 'insertstudent', component: InsertStudentComponent },
            { path: 'insertteacher', component: InsertTeacherComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
};
