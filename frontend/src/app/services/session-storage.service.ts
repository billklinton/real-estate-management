import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class SessionStorageService {
    constructor() { }

    getTokenFromSessionStorage() {
        return sessionStorage.getItem('token');
    }

    setTokenSessionStorage(value: string) {
        sessionStorage.setItem('token', value);
    }

    deleteTokenSessionStorage() {
        sessionStorage.removeItem('token');
    }
}
