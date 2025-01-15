import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './SurveyCreator.css';
import { AddButtonListSurveysWithId, AddButtonHRPanel } from './funcs/Buttons';
import { getUserRole, getEmployeeId } from './funcs/CookiesManager';

function SurveyCreator() {
    const [questionsArr, setQuestionsArr] = useState([]);
    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
    }, []);

    const [questionIndex, setQuestionIndex] = useState(-3);
    const [endDate, setEndDate] = useState(new Date());

    const handleDateChange = (event) => {
        setEndDate(event.target.value); 
    };

    function Selector() {
        return (
            <div className="container animate-fade-in">
                <div className="profile-buttons">
                    <button type="button" onClick={() => setQuestionIndex(-1)} className="selector-button button-editor animate-bounce">Редактор опросов</button>
                    <button type="button" onClick={() => createAutoSurvey()} className="selector-button button-auto animate-bounce">Автогенерация</button>
                    <input type="date" value={endDate || ''} onChange={handleDateChange} className="date-input" />
                </div>
            </div>
        );
    }

    const [surveyTitle, setSurveyTitle] = useState("");
    const handleSetSurveyTitle = (event) => {
        setSurveyTitle(event.target.value);
    };

    const [surveyDescription, setSurveyDescription] = useState("");
    const handleSetSurveyDescription = (event) => {
        setSurveyDescription(event.target.value);
    };

    function Start() {
        return (
            <div className="container animate-fade-in">
                <div>
                    <label> Название </label>
                    <input type="text" value={surveyTitle} required onChange={handleSetSurveyTitle}></input>
                </div>
                <div>
                    <label> Описание </label>
                    <input type="text" value={surveyDescription} required onChange={handleSetSurveyDescription}></input>
                </div>
                <div>
                    <button type="button" onClick={() => startQuestion()} className="list-surveys-btn button-margin animate-bounce"> Создание вопросов </button>
                </div>
            </div>
        );
    }

    const [questionText, setQuestionText] = useState('');
    const [answersTemp, setAnswersTemp] = useState([]);

    const handleSetQuestionText = (event) => {
        setQuestionText(event.target.value);
    };

    const [answerCount, setAnswerCount] = useState(0);
    const handleSetAnswerCount = (event) => {
        const count = event.target.value;
        setAnswerCount(count);
        setAnswersTemp(Array.from({ length: count }, () => ({ id: -1, text: '', score: 0 })));
    };

    const handleSetAnswerText = (event, index) => {
        const arr = answersTemp;
        arr[index].text = event.target.value;
        setAnswersTemp(arr);
    };
    const handleSetAnswerScore = (event, index) => {
        const arr = answersTemp;
        arr[index].score = event.target.value;
        setAnswersTemp(arr);
    };
    function Questions() {
        return (
            <div className="container  animate-fade-in">
                <div>
                    <label> Вопрос </label>
                    <input type="text" value={questionText} required onChange={handleSetQuestionText}></input>
                </div>
                <div>
                    <label> Количество ответов </label>
                    <input type="text" value={answerCount } required onChange={handleSetAnswerCount}></input>
                </div>
                <table>
                    <thead>
                        <tr>
                            <td>
                                <label> Ответ </label>
                            </td>
                            <td>
                                <label> Количество баллов </label>
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        {answersTemp.map((answer, index) =>
                            <tr key={index}>
                                <td>
                                    <input type="text" required onChange={(event) => handleSetAnswerText(event, index)}></input>
                                </td>
                                <td>
                                    <input type="text" required onChange={(event) => handleSetAnswerScore(event, index)}></input>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button type="button" onClick={() => createCustomSurvey()} className="list-surveys-btn button-editor button-margin animate-bounce"> Опубликовать опрос</button>
                <button type="button" onClick={() => nextQuestion()} className="list-surveys-btn button-auto button-margin animate-bounce"> Следующий вопрос</button>
            </div>
        );
    }

    function End() {
        return (
            <div className="container  animate-fade-in">
                <h2>Опрос сохранён</h2>
            </div>
        );
    }

    return (
        <>
            <div className="chart-container  animate-fade-in">
                <div className="profile-buttons">
                    {AddButtonListSurveysWithId(navigate, getEmployeeId())}
                    {AddButtonHRPanel(navigate)}
                </div>
            </div>
            {questionIndex === -3 && Selector()}
            {questionIndex === -1 && Start()}
            {questionIndex > -1 && Questions()}
            {questionIndex === -2 && End()}
        </>
    );

    async function startQuestion() {
        const newQuestionsArr = [{ id: -1, text: '', answers: [] }];
        setQuestionIndex(0);
        setQuestionText('');
        setQuestionsArr(newQuestionsArr);
    };

    async function nextQuestion() {
        const newQuestionsArr = [...questionsArr, { id: -1, text: '', answers: [] }];
        newQuestionsArr[questionIndex].answers = answersTemp;
        newQuestionsArr[questionIndex].text = questionText;
        setAnswerCount(0);
        setAnswersTemp([]);
        setQuestionIndex(questionIndex + 1);
        setQuestionText('');
        setQuestionsArr(newQuestionsArr);
    };

    async function createCustomSurvey() {
        const newQuestionsArr = questionsArr;
        newQuestionsArr[questionIndex].answers = answersTemp;
        newQuestionsArr[questionIndex].text = questionText;

        if (endDate === null) {
            return;
        }

        try {
            const response = await fetch('surveycreator', {
                method: 'POST',
                headers: {
                    "Content-type": "application/json",
                },
                body: JSON.stringify({
                    id: -1,
                    title: surveyTitle,
                    description: surveyDescription,
                    photoUrl: null,
                    creationData : new Date(),
                    endDate: endDate,
                    questions: newQuestionsArr,
                }),
            })

            if (response.ok) {
                setQuestionIndex(-2);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }

        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }

    }

    async function createAutoSurvey() {
        if (endDate === null) {
            return;
        }

        try {
            const response = await fetch('surveycreator/auto', {
                method: 'POST',
                headers: {
                    "Content-type": "application/json",
                },
                body: JSON.stringify(endDate),
            })

            if (response.ok) {
                setQuestionIndex(-2);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }

        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }

    }
}

export default SurveyCreator;