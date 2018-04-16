import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component({
    filters: {
        getTableNames(tables: any) { tables.map(function (f: any) { return f.name })} }
})
export default class TaskThirdStepComponent extends Vue {
    $route: any;
    $http: any;

    @Prop({ default: false })
    value!: boolean;
    
    ConfigurationID: number = 27;
    Tables: any = [];

    selectedTable: string = "";
    selectedColumn: string = "";
    selectedTableAction: string = "";
    selectedWhereColumn: string = "";
    

    tableActions: string[] = ["Clear the cell", "Hash the cell (MD5)", "Hash the cell (SHA-256)", "Remove the record"];
    periodSelections: any = [];
    selectedPeriod: number = 0;

    previous() {
        this.$emit('previous');
    }

    proceed() {
        this.$emit('proceed');
    }

    close() {
        this.$emit('close');
    }

    GetTables() {
        this.$http.get('Database/Tables', {
            params: {
                id: this.ConfigurationID
            }
        }).then((response: any) => {
            this.Tables = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    }

    tableNames() {
        return this.Tables.map(function (f: any) { return f.name; });
    }

    columnNames(tableName: string) {
        if (tableName.length > 0) {
            return this.Tables.filter(function (obj: any) {
                return obj.name == tableName;
            })[0].columns;
        } 

        return [];
    }

    generatePeriods() {
        for (var i = 1; i < 101; i++)
            this.periodSelections.push({ text: i + " Months", value: i });
    }

    mounted() {
        this.GetTables();
        this.generatePeriods();
    }
}
