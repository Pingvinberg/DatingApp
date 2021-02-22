import { Identifiers } from "@angular/compiler";

export interface Group {
    name: string;
    connections: Connections[];
}

interface Connections {
    connectionId: string;
    username: string;
}