import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import { Global } from '../../imports';
import { User } from '../models/user.model';

@Injectable()
export class UserService {
    //----------------PROPERTIRS-------------------

    basicURL: string = Global.BASE_ENDPOINT + `/user`;

    //----------------CONSTRUCTOR------------------

    constructor(private http: HttpClient) { }

    //----------------METHODS-------------------

    //POST
    signIn(userName: string, age: number): Observable<any> {
        let user:User=new User(userName,age);
        let url: string = `${this.basicURL}/signIn`;
        return this.http.post(url, user);
    }

    //GET
    getPartners(): Observable<any> {
        let url: string = `${this.basicURL}/getPartners`;
        return this.http.get(url);
    }

    //GET
    getUser(userName: string): Observable<any> {
        let url: string = `${this.basicURL}/getUser?userName=${userName}`;
        return this.http.get(url);
    }

    //PUT
    setPartner(currentUserName: string, partnerUserName: string): Observable<any> {
        let formData = new FormData();
        formData.append("currentUserName", currentUserName);
        formData.append("partnerUserName", partnerUserName);
        let url: string = `${this.basicURL}/setPartner`;
        return this.http.put(url, formData);
    }
}
