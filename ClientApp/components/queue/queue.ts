import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';
import moment from 'moment';

interface Application {
    ID: string;
    Name: string;
    Description: string;
    CreationDate: number;
    Tasks: any;
}


@Component({
    components: {
        MenuComponent: require('../navmenu/navmenu.vue.html')
    }
})
export default class QueueComponent extends Vue {
    $route: any;
    $http: any;
    EvaluationResults: any = {};

    @Prop({ default: false })
    value!: boolean;


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

    GetApplications() {
        this.$http.get('Database/EvaluateApplications', {
        }).then((response: any) => {
            this.EvaluationResults = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    };

    mounted() {
        this.GetApplications();
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

        headers.push({ text: "Action", value: "Action" });

        return headers;
    }
}
