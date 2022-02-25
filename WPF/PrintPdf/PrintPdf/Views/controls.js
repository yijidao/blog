const DataGrid = {
    props: ["itemsSource", "columns"],
    template: `
    <table style="width: 100%; border-collapse: collapse; border: 1px solid black; ">
        <thead>
            <tr>
                <th v-for="column in columns" style="border: 1px solid black; background-color: lightblue; height: 40px;">
                    {{column.Header}}
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="item in itemsSource">
                <td v-for="column in columns" style="text-align: center; vertical-align: middle; border: 1px solid black;  height: 32px;">
                    {{item[column.Binding]}}
                </td>
            </tr>
        </tbody>
    </table>
    `
}
const DocumentHeader = {
    props: ["title"],
    template: `
        <div style="width: 70%; height: 100px; margin: 0 auto; display: flex; align-items: center; justify-content: center;">
            <h2>{{title}}</h2>
        </div>
    `
};