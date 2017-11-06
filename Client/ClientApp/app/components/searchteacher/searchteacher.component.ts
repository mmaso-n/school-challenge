import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'searchteacher',
    template: `
        <h1>Search for Teachers</h1>
        <table cellpadding='2' cellspacing='2'>
            <tr>
                <td>Id:</td>
                <td><input #textbox type="number" [(ngModel)]="id" required> </td>
            </tr>
            <tr>
                <td>First Name:</td>
                <td><input #textbox type="text" [(ngModel)]="firstName" required> </td>
            </tr>
            <tr>
                <td>Last Name:</td>
                <td><input #textbox type="text" [(ngModel)]="lastName" required> </td>
            </tr>
        </table><br />
        <button (click)="searchTeacher()">Search Teacher</button>
<hr *ngIf="searchResults">
<h3 *ngIf="searchResults">Search Results</h3>
<table class='table' *ngIf="searchResults">
    <thead>
        <tr>
            <th>Id</th>
            <th>Name</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
        <tr *ngFor="let teacher of searchResults">
            <td>{{ teacher.id }}</td>
            <td>{{ teacher.lastName }}, {{ teacher.firstName }}</td>
        </tr>
    </tbody>
</table>
`
})
export class SearchTeacherComponent {
    public searchResults: Teacher[];
    public school = "";
    public id: number;
    public firstName = "";
    public lastName = "";
    public _http: Http;

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this._http = http;
    }

    doSearch() {
        let input = new FormData();

        if (this.id === undefined || this.id == null){
            input.append("teacherId", "");
        }
        else {
            input.append("teacherId", this.id.toString());
        }

        if (this.firstName === undefined || this.firstName == null){
            input.append("firstName", "");
        }
        else {
            input.append("firstName", this.firstName);
        }

        if (this.lastName === undefined || this.lastName == null) {
            input.append("lastName", "");
        }
        else {
            input.append("lastName", this.lastName);
        }
        
        return this._http
            .post("/api/teachers/SearchTeacherAsync/", input).subscribe(result => {
                this.searchResults = result.json() as Teacher[];
            });;
    }

    searchTeacher(): void {
        this.doSearch();
    }
}

interface Teacher {
    school: string;
    id: number;
    firstName: string;
    lastName: string
}
