export class User {
    constructor(
        public userName: string,
        public age: number,
        public partnerUserName: string=null,
        public score: number=0
    ) { }
}
