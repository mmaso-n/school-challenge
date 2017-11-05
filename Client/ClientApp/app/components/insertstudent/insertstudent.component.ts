import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'insertstudent',
    template: `
        <h1>Input New Student</h1>
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
        <button (click)="insertStudent()">Add Student</button>`
})
export class InsertStudentComponent {
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

    doInsert() {
        let input = new FormData();
        input.append("id", this.id.toString());
        input.append("number", this.number.toString());
        input.append("firstName", this.firstName);
        input.append("lastName", this.lastName);
        input.append("hasScholarship", String(this.hasScholarship));
        input.append("teacherId", this.teacherId.toString());

        return this._http
            .post("/api/students/InsertStudent/", input);
    }

    insertStudent(): void {
        this.doInsert()
            .subscribe(res => {
                console.log(res);
                location.reload(); // refresh display
            });
    }
}