import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'searchstudent',
    template: `
        <h1>Search for Students</h1>
        <table cellpadding='2' cellspacing='2'>
            <tr>
                <td>Id:</td>
                <td><input #textbox type="number" [(ngModel)]="id" required> </td>
            </tr>
            <tr>
                <td>Number:</td>
                <td><input #textbox type="text" [(ngModel)]="number" required> </td>
            </tr>
            <tr>
                <td>First Name:</td>
                <td><input #textbox type="text" [(ngModel)]="firstName" required> </td>
            </tr>
            <tr>
                <td>Last Name:</td>
                <td><input #textbox type="text" [(ngModel)]="lastName" required> </td>
            </tr>
            <tr>
                <td>Scholarship</td>
                <td>
                   <select [(ngModel)]="hasScholarship">
                    <option [ngValue]="false">false</option>
                     <option [ngValue]="true">true</option>
                   </select>
                </td>
            </tr>
            <tr>
                <td>TeacherId</td>
                <td><input #textbox type="number" [(ngModel)]="teacherId" required> </td>
            </tr>
        </table><br />
        <button (click)="searchStudent()">Search Student</button>
<hr *ngIf="searchResults">
<h3 *ngIf="searchResults">Search Results</h3>
<table class='table' *ngIf="searchResults">
    <thead>
        <tr>
            <th>Id</th>
            <th>Number</th>
            <th>Name</th>
            <th>Scholarship?</th>
            <th>TeacherId</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
        <tr *ngFor="let student of searchResults">
            <td>{{ student.id }}</td>
            <td>{{ student.number }}</td>
            <td>{{ student.lastName }}, {{ student.firstName }}</td>
            <td>{{ student.hasScholarship }}</td>
            <td>{{ student.teacherId }}</td>
        </tr>
    </tbody>
</table>
`
})
export class SearchStudentComponent {
    public searchResults: Student[];
    public school = "";
    public id: number;
    public number: number;
    public firstName = "";
    public lastName = "";
    public hasScholarship = true;
    public teacherId: number;
    public _http: Http;

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this._http = http;
    }

    doSearch() {
        let input = new FormData();

        if (this.id === undefined || this.id == null){
            input.append("studentId", "");
        }
        else {
            input.append("studentId", this.id.toString());
        }

        if (this.number === undefined || this.number == null)
        {
            input.append("studentNumber", "");
        }
        else {
            input.append("studentNumber", this.number.toString());
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

        if (this.teacherId === undefined || this.teacherId == null) {
            input.append("teacherId", "");
        }
        else {
            input.append("teacherId", this.teacherId.toString());
        }

        if (this.hasScholarship === undefined || this.hasScholarship == null) {
            input.append("hasScholarship", "");
        }
        else {
            input.append("hasScholarship", this.hasScholarship.toString());
        }

        return this._http
            .post("/api/students/SearchStudentAsync/", input).subscribe(result => {
                this.searchResults = result.json() as Student[];
            });;
    }

    searchStudent(): void {
        this.doSearch();
    }
}

interface Student {
    school: string;
    id: number;
    number: number;
    firstName: string;
    lastName: string;
    hasScholarship: boolean;
    teacherId: number
}
