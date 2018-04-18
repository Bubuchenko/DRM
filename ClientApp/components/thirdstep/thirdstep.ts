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

  ConfigurationID: number = 1;
  TableNames: string[] = [];
  TableData: any = [];

  inProgress: boolean = false;

  @Watch("selectedTable")
  onSelectedTableChanged(val: string, oldVal: string) {
    this.getTableData(5);
  }

  selectedTable: string = "";
  selectedColumn: string = "";
  selectedTableAction: string = "";
  selectedWhereColumn: string = "";

  tableActions: any[] = [
    { text: "Clear the cell", value: "NULL" },
    { text: "Hash the cell (MD5)", value: "MD5" },
    { text: "Hash the cell (SHA-256)", value: "SHA256" },
    { text: "Remove the record", value: "REMOVE" }
  ];

  selectedPeriod: number = 0;

  previous() {
    this.$emit("previous");
  }

  proceed() {
    this.$emit("proceed");
  }

  close() {
    this.$emit("close");
  }

  getTableNames(): void {
    this.$http
      .get("Database/Tables", {
        params: {
          id: this.ConfigurationID
        }
      })
      .then((response: any) => {
        this.TableNames = response.data;
      })
      .catch((error: any) => {
        alert(error);
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
        ConfigurationID: this.ConfigurationID,
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
      })
      .catch((error: any) => {
        this.inProgress = false;
        alert(error);
      });
  }

  columnNames() {
    if (this.TableData.length > 0) {
      var columns = (Object as any).getOwnPropertyNames(this.TableData[0]);
      columns.pop();
      return columns;
    }
  }

  periodSelections(): void {
    var periods: any = [];
    for (var i = 1; i < 101; i++)
      periods.push({ text: i + " Months", value: i });

    return periods;
  }

  mounted() {
    this.getTableNames();
  }

  getHeaders() {
    var columns = this.columnNames();
    var headers: any = [];

    for (var i = 0; i < columns.length; i++) {
      var header: any = { text: columns[i], value: columns[i] };
      headers.push(header);
    }

    return headers;
  }
}
