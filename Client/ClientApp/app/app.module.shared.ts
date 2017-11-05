import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { TeachersComponent } from './components/teachers/teachers.component';
import { UploadComponent } from './components/upload/upload.component';
import { PostComponent } from './components/post/post.component';

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        TeachersComponent,
        FetchDataComponent,
        HomeComponent,
        UploadComponent,
        PostComponent
    ],
    imports: [
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'teachers', component: TeachersComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'upload', component: UploadComponent },
            //{ path: 'blog/:id', component: PostComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
};
