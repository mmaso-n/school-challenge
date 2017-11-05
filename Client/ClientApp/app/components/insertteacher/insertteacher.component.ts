import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'insertteacher',
    template: `
        <h1>Add Teacher</h1>
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
        <button (click)="insertTeacher()">Add Teacher</button>`
})
export class InsertTeacherComponent {
    public school = "";
    public id: number;
    public firstName = "";
    public lastName = "";
    public _http: Http;

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this._http = http;
    }

    doInsert() {
        let input = new FormData();
        input.append("id", this.id.toString());
        input.append("firstName", this.firstName);
        input.append("lastName", this.lastName);

        return this._http
            .post("/api/teachers/InsertTeacher/", input);
    }

    insertTeacher(): void {
        this.doInsert()
            .subscribe(res => {
                console.log(res);
                location.reload(); // refresh display
            });
    }
}