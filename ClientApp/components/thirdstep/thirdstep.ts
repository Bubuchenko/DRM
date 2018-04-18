import Vue from "vue";
import { Component, Prop, Watch } from "vue-property-decorator";
import VueRouter from "vue-router";
import moment from "moment";

import CryptoJS from 'crypto-js';

@Component
export default class TaskThirdStepComponent extends Vue {
    $route: any;
    $http: any;

    @Prop({ default: false })
    value!: boolean;

    @Prop()
    configurationid!: number;

    TableNames: string[] = [];
    TableData: any = [];
    Columns: string[] = [];
    FilterColumns: string[] = [];
    PreviewItemCount: number = 50;

    UserIsFinished: boolean = false;

    inProgress: boolean = false;

    @Watch("selectedTable")
    onSelectedTableChanged(val: string, oldVal: string) {
        this.reset();
        this.getTableData(this.PreviewItemCount);
        this.getFilterColumns();
    }

    @Watch("configurationid")
    onConfigurationIDChanged(val: string, oldVal: string) {
        this.getTableNames();
    }


    @Watch("selectedPeriod")
    onSelectedPeriodChanged(val: string, oldVal: string) {
        this.getTableData(this.PreviewItemCount);
    }

    selectedTable: string = "";
    selectedColumn: string = "";
    selectedTableAction: string = "";
    selectedWhereColumn: string = "";
    selectedPeriod: number = 0;

    tableActions: any[] = [
        { text: "Clear the cell", value: "NULL" },
        { text: "Hash the cell (MD5)", value: "MD5" },
        { text: "Hash the cell (SHA-256)", value: "SHA256" },
        { text: "Remove the record", value: "REMOVE" }
    ];

    get setupIsComplete() {
        return this.selectedTable != "" &&
               this.selectedColumn != "" &&
               this.selectedTableAction != "" &&
               this.selectedWhereColumn != "" &&
               this.selectedPeriod != 0;
    }

    get setupCollection() {
        return {
            tableName: this.selectedTable,
            columnName: this.selectedColumn,
            type: this.selectedTableAction,
            filterColumn: this.selectedWhereColumn,
            periodInMonths: this.selectedPeriod
        };
    }

    previous() {
        this.$emit("previous");
    }

    proceed() {
        this.$emit("proceed", this.setupCollection);
    }

    close() {
        this.$emit("close");
    }

    reset() {
        this.selectedColumn = "";
        this.selectedTableAction = "";
        this.selectedWhereColumn = "";
    }

    getTableNames(): void {
        this.inProgress = true;
        this.$http
            .get("Database/Tables", {
                params: {
                    id: this.configurationid
                }
            })
            .then((response: any) => {
                this.TableNames = response.data;
                this.inProgress = false;
            })
            .catch((error: any) => {
                alert(error);
                this.inProgress = false;
            });
    }

    formatCellValue(value: any) {
        if (value.length > 30) value = value.substring(0, 30) + "...";

        if (moment(value).isValid() && !this.isInt(value))
            value = moment(value).format("DD-MM-YYYY");

        return value;
    }

    MD5(value: any) {
        return CryptoJS.MD5(value).toString();
    }

    SHA256(value: any): any {
        return CryptoJS.SHA256(value).toString();
    }

    isInt(value: any) {
        var x = parseFloat(value);
        return !isNaN(value) && (x | 0) === x;
    }

    getTableData(limit: number): void {
        this.inProgress = true;
        this.$http
            .post("Database/TableData", {
                ConfigurationID: this.configurationid,
                TableName: this.selectedTable,
                ActionColumn: this.selectedColumn,
                Action: this.selectedTableAction,
                FilterColumn: this.selectedWhereColumn,
                Months: this.selectedPeriod,
                Limit: limit
            })
            .then((response: any) => {
                this.inProgress = false;
                this.TableData = response.data;
                this.getColumnNames();
            })
            .catch((error: any) => {
                this.inProgress = false;
                alert(error);
            });
    }

    getFilterColumns(): void {
        this.inProgress = true;
        this.$http
            .post("Database/WhereColumns", {
                ConfigurationID: this.configurationid,
                TableName: this.selectedTable,
            })
            .then((response: any) => {
                this.inProgress = false;
                this.FilterColumns = response.data;
            })
            .catch((error: any) => {
                this.inProgress = false;
                alert(error);
            });
    }

    getColumnNames() {
        if (this.TableData.length > 0) {
            var columns = (Object as any).getOwnPropertyNames(this.TableData[0]);
            columns.pop();
            this.Columns = columns;
        }
    }


    periodSelections(): void {
        var periods: any = [];
        periods.push({ text: "Select a period", value: 0 });
        for (var i = 1; i < 101; i++)
            periods.push({ text: i + " Months", value: i });

        return periods;
    }


    getHeaders() {
        var headers: any = [];

        if (this.Columns == null)
            return;

        for (var i = 0; i < this.Columns.length; i++) {
            var header: any = { text: this.Columns[i], value: this.Columns[i] };
            headers.push(header);
        }

        return headers;
    }
}
