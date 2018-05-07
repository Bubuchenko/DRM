import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';
import moment from 'moment';
import CryptoJS from 'crypto-js';

interface Application {
    ID: string;
    Name: string;
    Description: string;
    CreationDate: number;
    Tasks: any;
}


@Component({
    components: {
        MenuComponent: require('../navmenu/navmenu.vue.html'),
        ExecuterComponent: require('../executer/executer.vue.html')
    }
})
export default class OrchestrationComponent extends Vue {
    $route: any;
    $http: any;
    EvaluationResults: any = {};

    @Prop({ default: false })
    value!: boolean;

    ExecutionDialog: boolean = false;
    userHasAgreed: boolean = false;
    ShowTaskExecuter: boolean = false;
    TaskExecuterParams: any = {};
    ConfigurationsParams: string[] = [];

    formatCellValue(value: any) {
        if (value.length > 30) value = value.substring(0, 30) + "...";

        if (moment(value).isValid() && !this.isInt(value))
            value = moment(value).format("DD-MM-YYYY");

        return value;
    }

    isInt(value: any) {
        var x = parseFloat(value);
        return !isNaN(value) && (x | 0) === x;
    }

    getConfigurations(): string[] {
        if (this.EvaluationResults.length > 0) {
            var configurations = [];
            for (var i = 0; i < this.EvaluationResults.length; i++) {
                for (var x = 0; x < this.EvaluationResults[i].configurations.length; x++) {
                    configurations.push(this.EvaluationResults[i].configurations[x]);
                }
            }
            return configurations;
        }

        return [];
    }

    executeActions() {
        this.ShowTaskExecuter = true;
        this.TaskExecuterParams = this.EvaluationResults;
        this.ConfigurationsParams = this.getConfigurations();
    }

    GetRecords() {
        this.$http.get('Database/AllNCRecords', {
        }).then((response: any) => {
            this.EvaluationResults = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    };

    mounted() {
        this.GetRecords();
    }

    generateColumns(rows: any) {
        var columns = (Object as any).getOwnPropertyNames(rows[0]);
        columns.pop();
        return columns;
    }

    generateHeaders(rows: any) {
        var headers: any = [];

        var columns = (Object as any).getOwnPropertyNames(rows[0]);
        columns.pop();

        for (var i = 0; i < columns.length; i++) {
            var header: any = { text: columns[i], value: columns[i] };
            headers.push(header);
        }

        return headers;
    }

    MD5(value: any) {
        return CryptoJS.MD5(value).toString();
    }

    SHA256(value: any): any {
        return CryptoJS.SHA256(value).toString();
    }
}
